using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class OrcamentoSuplementarDataManager : IOrcamentoSuplementarDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_REVIEW = "PENDING_REVIEW";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public OrcamentoSuplementarDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public OrcamentoSuplementarBatchResponse GetActiveBatch(GetActiveOrcamentoSuplementarRequest request)
        {
            OrcamentoSuplementarBatchResponse response = new OrcamentoSuplementarBatchResponse();
            try
            {
                OrcamentoSuplementar batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                response.Batch = MapBatch(batch);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private OrcamentoSuplementar GetOrCreateDraftBatch(int orcamentoConfigFk)
        {
            OrcamentoSuplementar batch = _unitOfWork.OrcamentoSuplementarRepository.GetActiveDraftBatch(orcamentoConfigFk);
            if (batch != null)
                return batch;

            batch = new OrcamentoSuplementar
            {
                OrcamentoConfigFk = orcamentoConfigFk,
                Estado = ESTADO_DRAFT,
                IndActivo = true
            };
            batch = (OrcamentoSuplementar)_utils.SetDetailsToEntity(batch);
            _unitOfWork.OrcamentoSuplementarRepository.Add(batch);
            _unitOfWork.Commit();
            return batch;
        }

        private OrcamentoSuplementarBatchDataContract MapBatch(OrcamentoSuplementar batch)
        {
            List<OrcamentoSuplementarLinha> linhas = _unitOfWork.OrcamentoSuplementarRepository.GetLinhasByBatch(batch.Id);

            return new OrcamentoSuplementarBatchDataContract
            {
                Id = batch.Id,
                OrcamentoConfigFk = batch.OrcamentoConfigFk,
                Estado = batch.Estado,
                SubmittedAt = batch.SubmittedAt,
                ReviewedAt = batch.ReviewedAt,
                ApprovedAt = batch.ApprovedAt,
                LastRejectComment = batch.LastRejectComment,
                LastRejectAt = batch.LastRejectAt,
                TotalAdjustment = linhas.Sum(l => l.AdjustmentValue),
                Linhas = linhas.Select(l => new OrcamentoSuplementarLinhaDataContract
                {
                    Id = l.Id,
                    OrcamentoLinhaFk = l.OrcamentoLinhaFk,
                    AtividadeCodigo = l.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Codigo,
                    AtividadeDesignacao = l.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Designacao,
                    EconomicClassificationCodigo = l.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Codigo,
                    EconomicClassificationDesignacao = l.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Designacao,
                    OrganizationNome = l.OrcamentoLinhaFkNavigation?.OrganizationFkNavigation?.Nome,
                    OldValue = l.OldValue,
                    AdjustmentValue = l.AdjustmentValue,
                    FinalValue = l.FinalValue
                }).ToList()
            };
        }

        public RubricasAprovadasParaSuplementarResponse GetRubricasAprovadas(GetRubricasAprovadasParaSuplementarRequest request)
        {
            RubricasAprovadasParaSuplementarResponse response = new RubricasAprovadasParaSuplementarResponse();
            try
            {
                response.Items = _unitOfWork.OrcamentoLinhaRepository.GetApprovedByOrcamentoConfig(request.OrcamentoConfigFk)
                    .Select(l => new RubricaAprovadaParaSuplementarDataContract
                    {
                        OrcamentoLinhaId = l.Id,
                        AtividadeCodigo = l.AtividadeFkNavigation?.Codigo,
                        AtividadeDesignacao = l.AtividadeFkNavigation?.Designacao,
                        EconomicClassificationCodigo = l.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = l.EconomicClassificationFkNavigation?.Designacao,
                        OrganizationNome = l.OrganizationFkNavigation?.Nome,
                        ValorAtual = l.Valor
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SaveLinha(SaveOrcamentoSuplementarLinhaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoSuplementar batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                if (batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-DRAFT", ErrorMessage = "Đợt bổ sung ngân sách đang chờ duyệt, không thể sửa." });
                    return response;
                }

                OrcamentoLinha rubrica = _unitOfWork.OrcamentoLinhaRepository.Get(request.OrcamentoLinhaFk);
                if (rubrica == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-RUBRICA-NOT-FOUND", ErrorMessage = "Không tìm thấy rúbrica." });
                    return response;
                }

                decimal oldValue = rubrica.Valor;
                decimal finalValue = oldValue + request.AdjustmentValue;
                if (finalValue < 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NEGATIVE", ErrorMessage = "Giá trị sau điều chỉnh không được âm." });
                    return response;
                }

                // Không cho giảm xuống dưới giá trị đã cam kết ở AD (nếu rúbrica đã
                // có AD) — bảo vệ phần đã cam kết chi ở bước sau, chỉ chặn khi GIẢM.
                if (request.AdjustmentValue < 0)
                {
                    ExpenditureAuthorization ad = _unitOfWork.ExpenditureAuthorizationRepository.GetByRubrica(request.OrcamentoLinhaFk);
                    if (ad != null)
                    {
                        decimal adValorRevisto = ad.ValorAutorizado + ad.Regularizacao;
                        if (finalValue < adValorRevisto)
                        {
                            response.Errors.Add(new Error { ErrorCode = "SUP-BELOW-COMMITTED", ErrorMessage = $"Không thể giảm xuống dưới giá trị đã cam kết ở AD ({adValorRevisto:N2})." });
                            return response;
                        }
                    }
                }

                OrcamentoSuplementarLinha entity = new OrcamentoSuplementarLinha
                {
                    Id = request.Id,
                    OrcamentoSuplementarFk = batch.Id,
                    OrcamentoLinhaFk = request.OrcamentoLinhaFk,
                    OldValue = oldValue,
                    AdjustmentValue = request.AdjustmentValue,
                    FinalValue = finalValue,
                    IndActivo = true
                };

                if (entity.Id > 0)
                {
                    entity = (OrcamentoSuplementarLinha)_utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.OrcamentoSuplementarRepository.UpdateLinha(entity);
                }
                else
                {
                    entity = (OrcamentoSuplementarLinha)_utils.SetDetailsToEntity(entity);
                    _unitOfWork.OrcamentoSuplementarRepository.AddLinha(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeleteLinha(DeleteOrcamentoSuplementarLinhaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoSuplementarLinha entity = _unitOfWork.OrcamentoSuplementarRepository.GetLinha(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng." });
                    return response;
                }

                OrcamentoSuplementar batch = _unitOfWork.OrcamentoSuplementarRepository.Get(entity.OrcamentoSuplementarFk);
                if (batch == null || batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-DRAFT", ErrorMessage = "Đợt bổ sung ngân sách đang chờ duyệt, không thể xoá dòng." });
                    return response;
                }

                entity.IndActivo = false;
                entity = (OrcamentoSuplementarLinha)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.OrcamentoSuplementarRepository.UpdateLinha(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitOrcamentoSuplementarRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoSuplementar batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                if (batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-DRAFT", ErrorMessage = "Đợt bổ sung ngân sách này không ở trạng thái nháp." });
                    return response;
                }

                List<OrcamentoSuplementarLinha> linhas = _unitOfWork.OrcamentoSuplementarRepository.GetLinhasByBatch(batch.Id);
                if (linhas.Count == 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-EMPTY", ErrorMessage = "Chưa có dòng điều chỉnh nào để gửi duyệt." });
                    return response;
                }

                batch.Estado = ESTADO_PENDING_REVIEW;
                batch.SubmittedBy = request.UserId;
                batch.SubmittedAt = DateTime.Now;
                batch = (OrcamentoSuplementar)_utils.UpdateDetailsToEntity(batch);
                _unitOfWork.OrcamentoSuplementarRepository.Update(batch);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Review(ReviewOrcamentoSuplementarRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoSuplementar batch = _unitOfWork.OrcamentoSuplementarRepository.Get(request.BatchId);
                if (batch == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-FOUND", ErrorMessage = "Không tìm thấy đợt bổ sung ngân sách." });
                    return response;
                }
                if (batch.Estado != ESTADO_PENDING_REVIEW)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-WRONG-STATE", ErrorMessage = "Đợt bổ sung ngân sách không ở trạng thái chờ kiểm tra." });
                    return response;
                }

                if (request.Approve)
                {
                    batch.Estado = ESTADO_PENDING_APPROVAL;
                    batch.ReviewedBy = request.UserId;
                    batch.ReviewedAt = DateTime.Now;
                }
                else
                {
                    batch.Estado = ESTADO_DRAFT;
                    batch.LastRejectBy = request.UserId;
                    batch.LastRejectAt = DateTime.Now;
                    batch.LastRejectComment = request.Comment;
                }

                batch = (OrcamentoSuplementar)_utils.UpdateDetailsToEntity(batch);
                _unitOfWork.OrcamentoSuplementarRepository.Update(batch);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveOrcamentoSuplementarRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoSuplementar batch = _unitOfWork.OrcamentoSuplementarRepository.Get(request.BatchId);
                if (batch == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-NOT-FOUND", ErrorMessage = "Không tìm thấy đợt bổ sung ngân sách." });
                    return response;
                }
                if (batch.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "SUP-WRONG-STATE", ErrorMessage = "Đợt bổ sung ngân sách không ở trạng thái chờ phê duyệt." });
                    return response;
                }

                if (request.Approve)
                {
                    batch.Estado = ESTADO_APPROVED;
                    batch.ApprovedBy = request.UserId;
                    batch.ApprovedAt = DateTime.Now;
                    batch = (OrcamentoSuplementar)_utils.UpdateDetailsToEntity(batch);
                    _unitOfWork.OrcamentoSuplementarRepository.Update(batch);

                    // Ghi FinalValue của từng dòng ngược lại OrcamentoLinha.Valor —
                    // giữ OrcamentoLinha là nguồn giá trị hiện tại DUY NHẤT cho mọi
                    // nơi đọc (AD, Cabimento, Compromisso, báo cáo), không cần logic
                    // "tìm điều chỉnh mới nhất" ở bất kỳ chỗ nào khác.
                    List<OrcamentoSuplementarLinha> linhas = _unitOfWork.OrcamentoSuplementarRepository.GetLinhasByBatch(batch.Id);
                    foreach (OrcamentoSuplementarLinha linha in linhas)
                    {
                        OrcamentoLinha rubrica = _unitOfWork.OrcamentoLinhaRepository.Get(linha.OrcamentoLinhaFk);
                        if (rubrica == null) { continue; }

                        rubrica.Valor = linha.FinalValue;
                        rubrica = (OrcamentoLinha)_utils.UpdateDetailsToEntity(rubrica);
                        _unitOfWork.OrcamentoLinhaRepository.Update(rubrica);
                    }
                }
                else
                {
                    batch.Estado = ESTADO_DRAFT;
                    batch.LastRejectBy = request.UserId;
                    batch.LastRejectAt = DateTime.Now;
                    batch.LastRejectComment = request.Comment;
                    batch = (OrcamentoSuplementar)_utils.UpdateDetailsToEntity(batch);
                    _unitOfWork.OrcamentoSuplementarRepository.Update(batch);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.ExcelReaderService;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class OrcamentoDataManager : IOrcamentoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_REVIEW = "PENDING_REVIEW";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public OrcamentoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public OrcamentoBatchResponse GetActiveBatch(GetActiveOrcamentoBatchRequest request)
        {
            OrcamentoBatchResponse response = new OrcamentoBatchResponse();
            try
            {
                OrcamentoBatch batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                response.Batch = MapBatch(batch);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        private OrcamentoBatch GetOrCreateDraftBatch(int orcamentoConfigFk)
        {
            OrcamentoBatch batch = _unitOfWork.OrcamentoBatchRepository.GetActiveDraftBatch(orcamentoConfigFk);
            if (batch != null)
                return batch;

            batch = new OrcamentoBatch
            {
                OrcamentoConfigFk = orcamentoConfigFk,
                Estado = ESTADO_DRAFT,
                IndActivo = true
            };
            batch = (OrcamentoBatch)_utils.SetDetailsToEntity(batch);
            _unitOfWork.OrcamentoBatchRepository.Add(batch);
            _unitOfWork.Commit();
            return batch;
        }

        private OrcamentoBatchDataContract MapBatch(OrcamentoBatch batch)
        {
            List<OrcamentoLinha> linhas = _unitOfWork.OrcamentoLinhaRepository.GetByBatch(batch.Id);

            OrcamentoBatchDataContract dto = new OrcamentoBatchDataContract
            {
                Id = batch.Id,
                OrcamentoConfigFk = batch.OrcamentoConfigFk,
                Estado = batch.Estado,
                SubmittedAt = batch.SubmittedAt,
                ReviewedAt = batch.ReviewedAt,
                ApprovedAt = batch.ApprovedAt,
                LastRejectComment = batch.LastRejectComment,
                LastRejectAt = batch.LastRejectAt,
                TotalValor = linhas.Sum(l => l.Valor),
                Linhas = linhas.Select(l => new OrcamentoLinhaDataContract
                {
                    Id = l.Id,
                    AtividadeFk = l.AtividadeFk,
                    AtividadeCodigo = l.AtividadeFkNavigation?.Codigo,
                    AtividadeDesignacao = l.AtividadeFkNavigation?.Designacao,
                    EconomicClassificationFk = l.EconomicClassificationFk,
                    EconomicClassificationCodigo = l.EconomicClassificationFkNavigation?.Codigo,
                    EconomicClassificationDesignacao = l.EconomicClassificationFkNavigation?.Designacao,
                    FunctionalClassificationFk = l.FunctionalClassificationFk,
                    FunctionalClassificationCodigo = l.FunctionalClassificationFkNavigation?.Codigo,
                    FunctionalClassificationDesignacao = l.FunctionalClassificationFkNavigation?.Designacao,
                    OrganizationFk = l.OrganizationFk,
                    OrganizationNome = l.OrganizationFkNavigation?.Nome,
                    Valor = l.Valor,
                    IndActivo = l.IndActivo
                }).ToList()
            };

            return dto;
        }

        public ResponseBaseDataContract SaveLinha(SaveOrcamentoLinhaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoBatch batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                if (batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-DRAFT", ErrorMessage = "Lô ngân sách đang chờ duyệt, không thể sửa. Vui lòng chờ kết quả duyệt." });
                    return response;
                }

                OrcamentoLinha entity = new OrcamentoLinha
                {
                    Id = request.Id,
                    OrcamentoBatchFk = batch.Id,
                    AtividadeFk = request.AtividadeFk,
                    EconomicClassificationFk = request.EconomicClassificationFk,
                    FunctionalClassificationFk = request.FunctionalClassificationFk,
                    OrganizationFk = request.OrganizationFk,
                    Valor = request.Valor,
                    IndActivo = true
                };

                if (!_unitOfWork.OrcamentoLinhaRepository.IsComboValid(entity))
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-DUP-RUBRICA", ErrorMessage = "Rúbrica này (Atividade + Classificação Económica + Organization) đã tồn tại." });
                    return response;
                }

                if (entity.Id > 0)
                {
                    entity = (OrcamentoLinha)_utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.OrcamentoLinhaRepository.Update(entity);
                }
                else
                {
                    entity = (OrcamentoLinha)_utils.SetDetailsToEntity(entity);
                    _unitOfWork.OrcamentoLinhaRepository.Add(entity);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeleteLinha(DeleteOrcamentoLinhaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoLinha entity = _unitOfWork.OrcamentoLinhaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng." });
                    return response;
                }

                OrcamentoBatch batch = _unitOfWork.OrcamentoBatchRepository.Get(entity.OrcamentoBatchFk);
                if (batch == null || batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-DRAFT", ErrorMessage = "Lô ngân sách đang chờ duyệt, không thể xoá dòng." });
                    return response;
                }

                entity.IndActivo = false;
                entity = (OrcamentoLinha)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.OrcamentoLinhaRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoBatch batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                if (batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-DRAFT", ErrorMessage = "Lô ngân sách này không ở trạng thái nháp." });
                    return response;
                }

                List<OrcamentoLinha> linhas = _unitOfWork.OrcamentoLinhaRepository.GetByBatch(batch.Id);
                if (linhas.Count == 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-EMPTY", ErrorMessage = "Chưa có dòng nào để gửi duyệt." });
                    return response;
                }

                batch.Estado = ESTADO_PENDING_REVIEW;
                batch.SubmittedBy = request.UserId;
                batch.SubmittedAt = DateTime.Now;
                batch = (OrcamentoBatch)_utils.UpdateDetailsToEntity(batch);
                _unitOfWork.OrcamentoBatchRepository.Update(batch);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Review(ReviewOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoBatch batch = _unitOfWork.OrcamentoBatchRepository.Get(request.BatchId);
                if (batch == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-FOUND", ErrorMessage = "Không tìm thấy lô ngân sách." });
                    return response;
                }
                if (batch.Estado != ESTADO_PENDING_REVIEW)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-WRONG-STATE", ErrorMessage = "Lô ngân sách không ở trạng thái chờ kiểm tra." });
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

                batch = (OrcamentoBatch)_utils.UpdateDetailsToEntity(batch);
                _unitOfWork.OrcamentoBatchRepository.Update(batch);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                OrcamentoBatch batch = _unitOfWork.OrcamentoBatchRepository.Get(request.BatchId);
                if (batch == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-FOUND", ErrorMessage = "Không tìm thấy lô ngân sách." });
                    return response;
                }
                if (batch.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-WRONG-STATE", ErrorMessage = "Lô ngân sách không ở trạng thái chờ phê duyệt." });
                    return response;
                }

                if (request.Approve)
                {
                    batch.Estado = ESTADO_APPROVED;
                    batch.ApprovedBy = request.UserId;
                    batch.ApprovedAt = DateTime.Now;
                }
                else
                {
                    batch.Estado = ESTADO_DRAFT;
                    batch.LastRejectBy = request.UserId;
                    batch.LastRejectAt = DateTime.Now;
                    batch.LastRejectComment = request.Comment;
                }

                batch = (OrcamentoBatch)_utils.UpdateDetailsToEntity(batch);
                _unitOfWork.OrcamentoBatchRepository.Update(batch);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ImportMasterDataTreeResponse Import(ImportOrcamentoRequest request)
        {
            ImportMasterDataTreeResponse response = new ImportMasterDataTreeResponse { RequestId = request.RequestId };
            try
            {
                OrcamentoBatch batch = GetOrCreateDraftBatch(request.OrcamentoConfigFk);
                if (batch.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "ORC-NOT-DRAFT", ErrorMessage = "Lô ngân sách đang chờ duyệt, không thể import thêm." });
                    return response;
                }

                List<Dictionary<string, string>> rows = MasterDataTreeExcelReader.ReadDataSheet(request.File);
                response.Total = rows.Count;

                for (int idx = 0; idx < rows.Count; idx++)
                {
                    int rowNum = idx + 3;
                    Dictionary<string, string> row = rows[idx];

                    row.TryGetValue("Atividade Código", out string atividadeCodigo);
                    row.TryGetValue("Classificação Económica Código", out string ecCodigo);
                    row.TryGetValue("Organization", out string organizationNome);
                    row.TryGetValue("Valor Orçamento Anual (USD)", out string valorTexto);

                    atividadeCodigo = atividadeCodigo?.Trim();
                    ecCodigo = ecCodigo?.Trim();
                    organizationNome = organizationNome?.Trim();

                    if (string.IsNullOrWhiteSpace(atividadeCodigo) || string.IsNullOrWhiteSpace(ecCodigo)
                        || string.IsNullOrWhiteSpace(organizationNome) || string.IsNullOrWhiteSpace(valorTexto))
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = atividadeCodigo, Message = "Thiếu Atividade/Classificação Económica/Organization/Valor." });
                        continue;
                    }

                    if (!decimal.TryParse(valorTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valor))
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = atividadeCodigo, Message = $"Valor '{valorTexto}' không hợp lệ." });
                        continue;
                    }

                    ProgramActivity atividade = _unitOfWork.ProgramActivityRepository.GetTreeByOrcamentoConfig(request.OrcamentoConfigFk)
                        .FirstOrDefault(a => a.Codigo == atividadeCodigo);
                    if (atividade == null)
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = atividadeCodigo, Message = $"Không tìm thấy Atividade '{atividadeCodigo}'." });
                        continue;
                    }

                    EconomicClassification econ = _unitOfWork.EconomicClassificationRepository.GetTreeByOrcamentoConfig(request.OrcamentoConfigFk)
                        .FirstOrDefault(e => e.Codigo == ecCodigo);
                    if (econ == null)
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = ecCodigo, Message = $"Không tìm thấy Classificação Económica '{ecCodigo}'." });
                        continue;
                    }

                    Institution organization = _unitOfWork.InstitutionRepository.GetAll()
                        .FirstOrDefault(i => string.Equals(i.Nome, organizationNome, StringComparison.OrdinalIgnoreCase));
                    if (organization == null)
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = organizationNome, Message = $"Không tìm thấy Organization '{organizationNome}'." });
                        continue;
                    }

                    OrcamentoLinha entity = new OrcamentoLinha
                    {
                        OrcamentoBatchFk = batch.Id,
                        AtividadeFk = atividade.Id,
                        EconomicClassificationFk = econ.Id,
                        OrganizationFk = organization.Id,
                        Valor = valor,
                        IndActivo = true
                    };

                    if (!_unitOfWork.OrcamentoLinhaRepository.IsComboValid(entity))
                    {
                        response.Failed++;
                        response.RowErrors.Add(new ImportRowError { Row = rowNum, Codigo = atividadeCodigo, Message = "Rúbrica này đã tồn tại." });
                        continue;
                    }

                    entity = (OrcamentoLinha)_utils.SetDetailsToEntity(entity);
                    _unitOfWork.OrcamentoLinhaRepository.Add(entity);
                    _unitOfWork.Commit();

                    response.Success++;
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}

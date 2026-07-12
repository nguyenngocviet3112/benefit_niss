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
    public class ExpenditureAuthorizationDataManager : IExpenditureAuthorizationDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_REVIEW = "PENDING_REVIEW";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public ExpenditureAuthorizationDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private ExpenditureAuthorizationDataContract MapEntity(ExpenditureAuthorization entity, decimal valorCabimentado = 0)
        {
            OrcamentoLinha rubrica = entity.OrcamentoLinhaFkNavigation;
            decimal valorRevisto = entity.ValorAutorizado + entity.Regularizacao;
            return new ExpenditureAuthorizationDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                OrcamentoLinhaFk = entity.OrcamentoLinhaFk,
                AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                AtividadeDesignacao = rubrica?.AtividadeFkNavigation?.Designacao,
                EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                OrganizationNome = rubrica?.OrganizationFkNavigation?.Nome,
                RubricaValor = rubrica?.Valor ?? 0,
                Descritivo = entity.Descritivo,
                ValorAutorizado = entity.ValorAutorizado,
                Regularizacao = entity.Regularizacao,
                ValorRevisto = valorRevisto,
                ValorCabimentado = valorCabimentado,
                SaldoDisponivel = valorRevisto - valorCabimentado,
                TipoDespesa = entity.TipoDespesa,
                SolicitaAberturaAprovisionamento = entity.SolicitaAberturaAprovisionamento,
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ReviewedAt = entity.ReviewedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt,
                Plurianualidade = (entity.ExpenditureAuthorizationPlurianualidade ?? new List<ExpenditureAuthorizationPlurianualidade>())
                    .Where(p => p.IndActivo)
                    .Select(p => new ExpenditureAuthorizationPlurianualidadeDataContract { Id = p.Id, Ano = p.Ano, Valor = p.Valor })
                    .OrderBy(p => p.Ano)
                    .ToList()
            };
        }

        public ExpenditureAuthorizationListResponse GetByAno(GetExpenditureAuthorizationListRequest request)
        {
            ExpenditureAuthorizationListResponse response = new ExpenditureAuthorizationListResponse();
            try
            {
                List<ExpenditureAuthorization> items = _unitOfWork.ExpenditureAuthorizationRepository.GetByAno(request.Ano);
                Dictionary<int, decimal> cabimentadoPorAd = _unitOfWork.ExpenditureAuthorizationRepository
                    .GetCabimentadoByAdIds(items.Select(a => a.Id).ToList());

                response.Items = items
                    .Select(a => MapEntity(a, cabimentadoPorAd.TryGetValue(a.Id, out var v) ? v : 0))
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public AvailableRubricasResponse GetAvailableRubricas(GetAvailableRubricasRequest request)
        {
            AvailableRubricasResponse response = new AvailableRubricasResponse();
            try
            {
                List<OrcamentoLinha> approved = _unitOfWork.OrcamentoLinhaRepository.GetApprovedByOrcamentoConfig(request.OrcamentoConfigFk);

                response.Items = approved
                    .Where(l => !_unitOfWork.ExpenditureAuthorizationRepository.HasAuthorizationForRubrica(l.Id))
                    .Select(l => new RubricaDisponivelDataContract
                    {
                        OrcamentoLinhaId = l.Id,
                        AtividadeCodigo = l.AtividadeFkNavigation?.Codigo,
                        AtividadeDesignacao = l.AtividadeFkNavigation?.Designacao,
                        EconomicClassificationCodigo = l.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = l.EconomicClassificationFkNavigation?.Designacao,
                        OrganizationNome = l.OrganizationFkNavigation?.Nome,
                        Valor = l.Valor
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ExpenditureAuthorizationResponse Create(CreateExpenditureAuthorizationRequest request)
        {
            ExpenditureAuthorizationResponse response = new ExpenditureAuthorizationResponse { RequestId = request.RequestId };
            try
            {
                if (_unitOfWork.ExpenditureAuthorizationRepository.HasAuthorizationForRubrica(request.OrcamentoLinhaFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-DUP-RUBRICA", ErrorMessage = "Rúbrica này đã có AD/Cabimento." });
                    return response;
                }

                int numero = _unitOfWork.ExpenditureAuthorizationRepository.GetNextNumero(request.Mes, request.Ano);

                ExpenditureAuthorization entity = new ExpenditureAuthorization
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    OrcamentoLinhaFk = request.OrcamentoLinhaFk,
                    Descritivo = request.Descritivo,
                    ValorAutorizado = request.ValorAutorizado,
                    Regularizacao = 0,
                    TipoDespesa = request.TipoDespesa,
                    SolicitaAberturaAprovisionamento = request.SolicitaAberturaAprovisionamento,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.Add(entity);
                _unitOfWork.Commit();

                ExpenditureAuthorization created = _unitOfWork.ExpenditureAuthorizationRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Save(SaveExpenditureAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization entity = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-FOUND", ErrorMessage = "Không tìm thấy AD." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-DRAFT", ErrorMessage = "AD đang chờ duyệt, không thể sửa." });
                    return response;
                }

                entity.Descritivo = request.Descritivo;
                entity.ValorAutorizado = request.ValorAutorizado;
                entity.Regularizacao = request.Regularizacao;
                entity.TipoDespesa = request.TipoDespesa;
                entity.SolicitaAberturaAprovisionamento = request.SolicitaAberturaAprovisionamento;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SavePlurianualidade(SavePlurianualidadeRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization parent = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.ExpenditureAuthorizationFk);
                if (parent == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-FOUND", ErrorMessage = "Không tìm thấy AD." });
                    return response;
                }
                if (parent.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-DRAFT", ErrorMessage = "AD đang chờ duyệt, không thể sửa Plurianualidade." });
                    return response;
                }

                ExpenditureAuthorizationPlurianualidade entity = new ExpenditureAuthorizationPlurianualidade
                {
                    Id = request.Id,
                    ExpenditureAuthorizationFk = request.ExpenditureAuthorizationFk,
                    Ano = request.Ano,
                    Valor = request.Valor,
                    IndActivo = true
                };

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.ExpenditureAuthorizationRepository.UpdatePlurianualidade(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.ExpenditureAuthorizationRepository.AddPlurianualidade(entity);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeletePlurianualidade(DeletePlurianualidadeRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorizationPlurianualidade entity = _unitOfWork.ExpenditureAuthorizationRepository.GetPlurianualidade(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-PLURI-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng Plurianualidade." });
                    return response;
                }

                entity.IndActivo = false;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.UpdatePlurianualidade(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitExpenditureAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization entity = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-FOUND", ErrorMessage = "Không tìm thấy AD." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-DRAFT", ErrorMessage = "AD không ở trạng thái nháp." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_REVIEW;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Review(ReviewExpenditureAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization entity = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-FOUND", ErrorMessage = "Không tìm thấy AD." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_REVIEW)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-WRONG-STATE", ErrorMessage = "AD không ở trạng thái chờ kiểm tra." });
                    return response;
                }

                if (request.Approve)
                {
                    entity.Estado = ESTADO_PENDING_APPROVAL;
                    entity.ReviewedBy = request.UserId;
                    entity.ReviewedAt = DateTime.Now;
                }
                else
                {
                    entity.Estado = ESTADO_DRAFT;
                    entity.LastRejectBy = request.UserId;
                    entity.LastRejectAt = DateTime.Now;
                    entity.LastRejectComment = request.Comment;
                }

                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveExpenditureAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ExpenditureAuthorization entity = _unitOfWork.ExpenditureAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-NOT-FOUND", ErrorMessage = "Không tìm thấy AD." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "AD-WRONG-STATE", ErrorMessage = "AD không ở trạng thái chờ phê duyệt." });
                    return response;
                }

                if (request.Approve)
                {
                    entity.Estado = ESTADO_APPROVED;
                    entity.ApprovedBy = request.UserId;
                    entity.ApprovedAt = DateTime.Now;
                }
                else
                {
                    entity.Estado = ESTADO_DRAFT;
                    entity.LastRejectBy = request.UserId;
                    entity.LastRejectAt = DateTime.Now;
                    entity.LastRejectComment = request.Comment;
                }

                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ExpenditureAuthorizationRepository.Update(entity);
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

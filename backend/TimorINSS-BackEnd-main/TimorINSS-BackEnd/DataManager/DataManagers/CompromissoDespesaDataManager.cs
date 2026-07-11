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
    public class CompromissoDespesaDataManager : ICompromissoDespesaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_REVIEW = "PENDING_REVIEW";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public CompromissoDespesaDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private CompromissoDespesaDataContract MapEntity(CompromissoDespesa entity)
        {
            Cabimento cabimento = entity.CabimentoFkNavigation;
            ExpenditureAuthorization ad = cabimento?.ExpenditureAuthorizationFkNavigation;
            OrcamentoLinha rubrica = ad?.OrcamentoLinhaFkNavigation;

            return new CompromissoDespesaDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                CabimentoFk = entity.CabimentoFk,
                CabimentoNumero = cabimento?.Numero ?? 0,
                ExpenditureAuthorizationNumero = ad?.Numero ?? 0,
                AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                AtividadeDesignacao = rubrica?.AtividadeFkNavigation?.Designacao,
                EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                OrganizationNome = rubrica?.OrganizationFkNavigation?.Nome,
                ValorCabimentado = cabimento?.ValorCabimentado ?? 0,
                Descritivo = entity.Descritivo,
                ValorCompromissoGlobal = entity.ValorCompromissoGlobal,
                ValorCompromissoAno = entity.ValorCompromissoAno,
                Regularizacao = entity.Regularizacao,
                ValorRevisto = entity.ValorCompromissoAno + entity.Regularizacao,
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ReviewedAt = entity.ReviewedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt,
                Plurianualidade = (entity.CompromissoDespesaPlurianualidade ?? new List<CompromissoDespesaPlurianualidade>())
                    .Where(p => p.IndActivo)
                    .Select(p => new CompromissoDespesaPlurianualidadeDataContract { Id = p.Id, Ano = p.Ano, Valor = p.Valor })
                    .OrderBy(p => p.Ano)
                    .ToList()
            };
        }

        public CompromissoDespesaListResponse GetByAno(GetCompromissoDespesaListRequest request)
        {
            CompromissoDespesaListResponse response = new CompromissoDespesaListResponse();
            try
            {
                response.Items = _unitOfWork.CompromissoDespesaRepository.GetByAno(request.Ano)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CabimentosDisponiveisResponse GetCabimentosDisponiveis(GetCabimentosDisponiveisRequest request)
        {
            CabimentosDisponiveisResponse response = new CabimentosDisponiveisResponse();
            try
            {
                response.Items = _unitOfWork.CabimentoRepository.GetByAno(request.Ano)
                    .Where(c => c.Estado == "APPROVED")
                    .Select(c => new CabimentoDisponivelParaCompromissoDataContract
                    {
                        CabimentoId = c.Id,
                        Numero = c.Numero,
                        AtividadeCodigo = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Codigo,
                        AtividadeDesignacao = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation?.AtividadeFkNavigation?.Designacao,
                        EconomicClassificationCodigo = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation?.EconomicClassificationFkNavigation?.Designacao,
                        OrganizationNome = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation?.OrganizationFkNavigation?.Nome,
                        ValorCabimentado = c.ValorCabimentado
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CompromissoDespesaResponse Create(CreateCompromissoDespesaRequest request)
        {
            CompromissoDespesaResponse response = new CompromissoDespesaResponse { RequestId = request.RequestId };
            try
            {
                Cabimento cabimento = _unitOfWork.CabimentoRepository.Get(request.CabimentoFk);
                if (cabimento == null || cabimento.Estado != "APPROVED")
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-CAB-NOT-APPROVED", ErrorMessage = "Cabimento phải ở trạng thái đã duyệt trước khi tạo Compromisso." });
                    return response;
                }

                int numero = _unitOfWork.CompromissoDespesaRepository.GetNextNumero(request.Mes, request.Ano);

                CompromissoDespesa entity = new CompromissoDespesa
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    CabimentoFk = request.CabimentoFk,
                    Descritivo = request.Descritivo,
                    ValorCompromissoGlobal = request.ValorCompromissoGlobal,
                    ValorCompromissoAno = request.ValorCompromissoAno,
                    Regularizacao = 0,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.CompromissoDespesaRepository.Add(entity);
                _unitOfWork.Commit();

                CompromissoDespesa created = _unitOfWork.CompromissoDespesaRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract SavePlurianualidade(SaveCompromissoDespesaPlurianualidadeRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                CompromissoDespesa parent = _unitOfWork.CompromissoDespesaRepository.Get(request.CompromissoDespesaFk);
                if (parent == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-FOUND", ErrorMessage = "Không tìm thấy Compromisso." });
                    return response;
                }
                if (parent.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-DRAFT", ErrorMessage = "Compromisso đang chờ duyệt, không thể sửa Plurianualidade." });
                    return response;
                }

                CompromissoDespesaPlurianualidade entity = new CompromissoDespesaPlurianualidade
                {
                    Id = request.Id,
                    CompromissoDespesaFk = request.CompromissoDespesaFk,
                    Ano = request.Ano,
                    Valor = request.Valor,
                    IndActivo = true
                };

                if (entity.Id > 0)
                {
                    entity = _utils.UpdateDetailsToEntity(entity);
                    _unitOfWork.CompromissoDespesaRepository.UpdatePlurianualidade(entity);
                }
                else
                {
                    entity = _utils.SetDetailsToEntity(entity);
                    _unitOfWork.CompromissoDespesaRepository.AddPlurianualidade(entity);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitCompromissoDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                CompromissoDespesa entity = _unitOfWork.CompromissoDespesaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-FOUND", ErrorMessage = "Không tìm thấy Compromisso." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-DRAFT", ErrorMessage = "Compromisso không ở trạng thái nháp." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_REVIEW;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.CompromissoDespesaRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Review(ReviewCompromissoDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                CompromissoDespesa entity = _unitOfWork.CompromissoDespesaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-FOUND", ErrorMessage = "Không tìm thấy Compromisso." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_REVIEW)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-WRONG-STATE", ErrorMessage = "Compromisso không ở trạng thái chờ kiểm tra." });
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
                _unitOfWork.CompromissoDespesaRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveCompromissoDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                CompromissoDespesa entity = _unitOfWork.CompromissoDespesaRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-NOT-FOUND", ErrorMessage = "Không tìm thấy Compromisso." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "COMP-WRONG-STATE", ErrorMessage = "Compromisso không ở trạng thái chờ phê duyệt." });
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
                _unitOfWork.CompromissoDespesaRepository.Update(entity);
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

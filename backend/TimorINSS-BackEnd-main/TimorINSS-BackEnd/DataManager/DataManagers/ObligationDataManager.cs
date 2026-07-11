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
    public class ObligationDataManager : IObligationDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public ObligationDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private decimal SaldoDisponivel(CompromissoDespesa compromisso)
        {
            decimal valorRevisto = compromisso.ValorCompromissoAno + compromisso.Regularizacao;
            decimal jaComprometido = _unitOfWork.ObligationRepository.GetTotalCommittedForCompromisso(compromisso.Id);
            return valorRevisto - jaComprometido;
        }

        private ObligationDataContract MapEntity(Obligation entity)
        {
            List<ObligationItemDataContract> items = (entity.ObligationItem ?? new List<ObligationItem>())
                .Where(i => i.IndActivo)
                .Select(i =>
                {
                    CompromissoDespesa comp = i.CompromissoDespesaFkNavigation;
                    OrcamentoLinha rubrica = comp?.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                    return new ObligationItemDataContract
                    {
                        Id = i.Id,
                        CompromissoDespesaFk = i.CompromissoDespesaFk,
                        CompromissoDespesaNumero = comp?.Numero ?? 0,
                        AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                        EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                        EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                        CompromissoValorRevisto = comp != null ? comp.ValorCompromissoAno + comp.Regularizacao : 0,
                        CompromissoSaldoDisponivel = comp != null ? SaldoDisponivel(comp) + i.Value : 0,
                        Value = i.Value
                    };
                })
                .ToList();

            return new ObligationDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                DescritivoObrigacao = entity.DescritivoObrigacao,
                ValorObrigacao = items.Sum(i => i.Value),
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt,
                Items = items
            };
        }

        public ObligationListResponse GetByAno(GetObligationListRequest request)
        {
            ObligationListResponse response = new ObligationListResponse();
            try
            {
                response.Items = _unitOfWork.ObligationRepository.GetByAno(request.Ano)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CompromissosComSaldoResponse GetCompromissosComSaldo(GetCompromissosComSaldoRequest request)
        {
            CompromissosComSaldoResponse response = new CompromissosComSaldoResponse();
            try
            {
                List<CompromissoDespesa> approved = _unitOfWork.CompromissoDespesaRepository.GetByAno(request.Ano)
                    .Where(c => c.Estado == "APPROVED")
                    .ToList();

                response.Items = approved
                    .Select(c =>
                    {
                        OrcamentoLinha rubrica = c.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                        return new CompromissoComSaldoDataContract
                        {
                            CompromissoDespesaId = c.Id,
                            Numero = c.Numero,
                            AtividadeCodigo = rubrica?.AtividadeFkNavigation?.Codigo,
                            AtividadeDesignacao = rubrica?.AtividadeFkNavigation?.Designacao,
                            EconomicClassificationCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                            EconomicClassificationDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                            OrganizationNome = rubrica?.OrganizationFkNavigation?.Nome,
                            ValorRevisto = c.ValorCompromissoAno + c.Regularizacao,
                            SaldoDisponivel = SaldoDisponivel(c)
                        };
                    })
                    .Where(c => c.SaldoDisponivel > 0)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ObligationResponse Create(CreateObligationRequest request)
        {
            ObligationResponse response = new ObligationResponse { RequestId = request.RequestId };
            try
            {
                int numero = _unitOfWork.ObligationRepository.GetNextNumero(request.Mes, request.Ano);

                Obligation entity = new Obligation
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    DescritivoObrigacao = request.DescritivoObrigacao,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.ObligationRepository.Add(entity);
                _unitOfWork.Commit();

                Obligation created = _unitOfWork.ObligationRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract AddItem(AddObligationItemRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation obligation = _unitOfWork.ObligationRepository.Get(request.ObligationFk);
                if (obligation == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể sửa." });
                    return response;
                }

                CompromissoDespesa compromisso = _unitOfWork.CompromissoDespesaRepository.Get(request.CompromissoDespesaFk);
                if (compromisso == null || compromisso.Estado != "APPROVED")
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-COMP-NOT-APPROVED", ErrorMessage = "Compromisso phải ở trạng thái đã duyệt." });
                    return response;
                }

                decimal saldo = SaldoDisponivel(compromisso);
                if (request.Value > saldo)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-EXCEEDS-SALDO", ErrorMessage = $"Giá trị vượt quá saldo còn lại của Compromisso ({saldo:N2})." });
                    return response;
                }
                if (request.Value <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-INVALID-VALUE", ErrorMessage = "Giá trị phải lớn hơn 0." });
                    return response;
                }

                ObligationItem item = new ObligationItem
                {
                    ObligationFk = request.ObligationFk,
                    CompromissoDespesaFk = request.CompromissoDespesaFk,
                    Value = request.Value,
                    IndActivo = true
                };
                item = _utils.SetDetailsToEntity(item);
                _unitOfWork.ObligationRepository.AddItem(item);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract RemoveItem(RemoveObligationItemRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                ObligationItem item = _unitOfWork.ObligationRepository.GetItem(request.Id);
                if (item == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-ITEM-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng." });
                    return response;
                }

                Obligation obligation = _unitOfWork.ObligationRepository.Get(item.ObligationFk);
                if (obligation == null || obligation.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação đang chờ duyệt, không thể xoá dòng." });
                    return response;
                }

                item.IndActivo = false;
                item = _utils.UpdateDetailsToEntity(item);
                _unitOfWork.ObligationRepository.UpdateItem(item);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitObligationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation entity = _unitOfWork.ObligationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-DRAFT", ErrorMessage = "Obrigação không ở trạng thái nháp." });
                    return response;
                }
                if (!entity.ObligationItem.Any(i => i.IndActivo))
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-EMPTY", ErrorMessage = "Chưa có dòng Compromisso nào để gửi duyệt." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_APPROVAL;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.ObligationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApproveObligationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                Obligation entity = _unitOfWork.ObligationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-NOT-FOUND", ErrorMessage = "Không tìm thấy Obrigação." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "OBR-WRONG-STATE", ErrorMessage = "Obrigação không ở trạng thái chờ phê duyệt." });
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
                _unitOfWork.ObligationRepository.Update(entity);
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

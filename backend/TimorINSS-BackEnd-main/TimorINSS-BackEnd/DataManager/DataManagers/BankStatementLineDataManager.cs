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
    public class BankStatementLineDataManager : IBankStatementLineDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public BankStatementLineDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        private BankStatementLineDataContract MapEntity(BankStatementLine entity)
        {
            return new BankStatementLineDataContract
            {
                Id = entity.Id,
                ContaBancariaFk = entity.ContaBancariaFk,
                ContaBancariaNome = entity.ContaBancariaFkNavigation != null
                    ? $"{entity.ContaBancariaFkNavigation.EntidadeBancaria} ({entity.ContaBancariaFkNavigation.Numero})"
                    : null,
                DataValor = entity.DataValor,
                Descricao = entity.Descricao,
                Credito = entity.Credito,
                Debito = entity.Debito,
                IsConciliado = entity.ReceitaPacFk.HasValue || entity.PaymentExecutionFk.HasValue,
                ReceitaPacFk = entity.ReceitaPacFk,
                ReceitaPacDescricao = entity.ReceitaPacFkNavigation != null
                    ? $"Receita {entity.ReceitaPacFkNavigation.Numero} — {entity.ReceitaPacFkNavigation.Descritivo}"
                    : null,
                PaymentExecutionFk = entity.PaymentExecutionFk,
                PaymentExecutionDescricao = entity.PaymentExecutionFkNavigation?.PaymentAuthorizationFkNavigation?.ObligationFkNavigation != null
                    ? $"Pagamento Obr {entity.PaymentExecutionFkNavigation.PaymentAuthorizationFkNavigation.ObligationFkNavigation.Numero}"
                    : null,
                ConciliadoAt = entity.ConciliadoAt
            };
        }

        public BankStatementLineListResponse GetByContaBancaria(GetBankStatementLinesRequest request)
        {
            BankStatementLineListResponse response = new BankStatementLineListResponse();
            try
            {
                response.Items = _unitOfWork.BankStatementLineRepository.GetByContaBancaria(request.ContaBancariaFk)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ReceitasDisponiveisParaConciliacaoResponse GetReceitasDisponiveis(GetReceitasDisponiveisParaConciliacaoRequest request)
        {
            ReceitasDisponiveisParaConciliacaoResponse response = new ReceitasDisponiveisParaConciliacaoResponse();
            try
            {
                response.Items = _unitOfWork.ReceitaPacRepository.GetByAno(request.Ano)
                    .Where(r => !_unitOfWork.BankStatementLineRepository.HasLineForReceita(r.Id))
                    .Select(r => new ReceitaDisponivelParaConciliacaoDataContract
                    {
                        ReceitaPacId = r.Id,
                        Numero = r.Numero,
                        Mes = r.Mes,
                        Ano = r.Ano,
                        Descritivo = r.Descritivo,
                        ValorPac = r.ValorPac
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public PagamentosDisponiveisParaConciliacaoResponse GetPagamentosDisponiveis()
        {
            PagamentosDisponiveisParaConciliacaoResponse response = new PagamentosDisponiveisParaConciliacaoResponse();
            try
            {
                response.Items = _unitOfWork.PaymentExecutionRepository.GetAll()
                    .Where(e => !_unitOfWork.BankStatementLineRepository.HasLineForPaymentExecution(e.Id))
                    .Select(e => new PagamentoDisponivelParaConciliacaoDataContract
                    {
                        PaymentExecutionId = e.Id,
                        ObligationNumero = e.PaymentAuthorizationFkNavigation?.ObligationFkNavigation?.Numero ?? 0,
                        ObligationDescritivo = e.PaymentAuthorizationFkNavigation?.ObligationFkNavigation?.DescritivoObrigacao,
                        DataPagamento = e.DataPagamento,
                        NumeroDocumento = e.NumeroDocumento,
                        ValorAutorizado = e.PaymentAuthorizationFkNavigation?.ValorAutorizado ?? 0
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract AddLine(AddBankStatementLineRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                if (request.Credito <= 0 && request.Debito <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-INVALID-VALUE", ErrorMessage = "Cần nhập Crédito hoặc Débito lớn hơn 0." });
                    return response;
                }

                BankStatementLine entity = new BankStatementLine
                {
                    ContaBancariaFk = request.ContaBancariaFk,
                    DataValor = request.DataValor,
                    Descricao = request.Descricao,
                    Credito = request.Credito,
                    Debito = request.Debito,
                    IndActivo = true
                };
                entity = (BankStatementLine)_utils.SetDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Add(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract DeleteLine(DeleteBankStatementLineRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                BankStatementLine entity = _unitOfWork.BankStatementLineRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng sao kê." });
                    return response;
                }
                if (entity.ReceitaPacFk.HasValue || entity.PaymentExecutionFk.HasValue)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-CONCILIADO", ErrorMessage = "Dòng đã đối chiếu, cần hủy đối chiếu trước khi xoá." });
                    return response;
                }

                entity.IndActivo = false;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract MatchReceita(MatchReceitaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                BankStatementLine entity = _unitOfWork.BankStatementLineRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng sao kê." });
                    return response;
                }
                if (entity.ReceitaPacFk.HasValue || entity.PaymentExecutionFk.HasValue)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-ALREADY-MATCHED", ErrorMessage = "Dòng này đã được đối chiếu." });
                    return response;
                }
                if (_unitOfWork.BankStatementLineRepository.HasLineForReceita(request.ReceitaPacFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-RECEITA-ALREADY-MATCHED", ErrorMessage = "Receita này đã được đối chiếu với dòng khác." });
                    return response;
                }

                entity.ReceitaPacFk = request.ReceitaPacFk;
                entity.ConciliadoBy = request.UserId;
                entity.ConciliadoAt = DateTime.Now;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract MatchPagamento(MatchPagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                BankStatementLine entity = _unitOfWork.BankStatementLineRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng sao kê." });
                    return response;
                }
                if (entity.ReceitaPacFk.HasValue || entity.PaymentExecutionFk.HasValue)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-ALREADY-MATCHED", ErrorMessage = "Dòng này đã được đối chiếu." });
                    return response;
                }
                if (_unitOfWork.BankStatementLineRepository.HasLineForPaymentExecution(request.PaymentExecutionFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-PAGAMENTO-ALREADY-MATCHED", ErrorMessage = "Pagamento này đã được đối chiếu với dòng khác." });
                    return response;
                }

                entity.PaymentExecutionFk = request.PaymentExecutionFk;
                entity.ConciliadoBy = request.UserId;
                entity.ConciliadoAt = DateTime.Now;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Unmatch(UnmatchRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                BankStatementLine entity = _unitOfWork.BankStatementLineRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-NOT-FOUND", ErrorMessage = "Không tìm thấy dòng sao kê." });
                    return response;
                }

                entity.ReceitaPacFk = null;
                entity.PaymentExecutionFk = null;
                entity.ConciliadoBy = null;
                entity.ConciliadoAt = null;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);
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

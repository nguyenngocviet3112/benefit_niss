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
    public class PaymentDataManager : IPaymentDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly ILancamentoDataManager _lancamentoDataManager;

        private const string ESTADO_DRAFT = "DRAFT";
        private const string ESTADO_PENDING_APPROVAL = "PENDING_APPROVAL";
        private const string ESTADO_APPROVED = "APPROVED";

        public PaymentDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, ILancamentoDataManager lancamentoDataManager)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _lancamentoDataManager = lancamentoDataManager;
        }

        private static decimal ValorObrigacao(Obligation obligation)
        {
            return obligation?.ObligationItem?.Where(i => i.IndActivo).Sum(i => i.Value) ?? 0;
        }

        private PaymentAuthorizationDataContract MapEntity(PaymentAuthorization entity)
        {
            return new PaymentAuthorizationDataContract
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Mes = entity.Mes,
                Ano = entity.Ano,
                ObligationFk = entity.ObligationFk,
                ObligationNumero = entity.ObligationFkNavigation?.Numero ?? 0,
                ObligationDescritivo = entity.ObligationFkNavigation?.DescritivoObrigacao,
                ValorObrigacao = ValorObrigacao(entity.ObligationFkNavigation),
                BeneficiarioNome = entity.ObligationFkNavigation?.BeneficiarioNome,
                BeneficiarioCategoria = entity.ObligationFkNavigation?.BeneficiarioCategoria,
                BeneficiarioNomeConta = entity.ObligationFkNavigation?.BeneficiarioNomeConta,
                BeneficiarioNumeroConta = entity.ObligationFkNavigation?.BeneficiarioNumeroConta,
                BeneficiarioIban = entity.ObligationFkNavigation?.BeneficiarioIban,
                BeneficiarioSwift = entity.ObligationFkNavigation?.BeneficiarioSwift,
                BeneficiarioBanco = entity.ObligationFkNavigation?.BeneficiarioBanco,
                BeneficiarioMontanteAPagar = entity.ObligationFkNavigation?.BeneficiarioMontanteAPagar,
                BeneficiaryList = (entity.ObligationFkNavigation?.ObligationBeneficiary ?? new List<ObligationBeneficiary>())
                    .Where(b => b.IndActivo)
                    .Select(b => new ObligationBeneficiaryDataContract
                    {
                        Id = b.Id,
                        Niss = b.Niss,
                        NomeContribuinte = b.NomeContribuinte,
                        NomeBeneficiario = b.NomeBeneficiario,
                        NomeConta = b.NomeConta,
                        NumeroConta = b.NumeroConta,
                        Iban = b.Iban,
                        Swift = b.Swift,
                        Banco = b.Banco,
                        SalarioIliquido = b.SalarioIliquido,
                        Cotizacao4 = b.Cotizacao4,
                        Imposto10 = b.Imposto10,
                        SalarioLiquido = b.SalarioLiquido,
                        OutrosSuplementos = b.OutrosSuplementos,
                        MontanteAPagar = b.MontanteAPagar
                    })
                    .ToList(),
                Descritivo = entity.Descritivo,
                ValorAutorizado = entity.ValorAutorizado,
                CodigoContaDebitoFk = entity.CodigoContaDebitoFk,
                CodigoContaDebitoDesignacao = entity.CodigoContaDebitoFkNavigation?.Designacao,
                CodigoContaCreditoFk = entity.CodigoContaCreditoFk,
                CodigoContaCreditoDesignacao = entity.CodigoContaCreditoFkNavigation?.Designacao,
                Estado = entity.Estado,
                SubmittedAt = entity.SubmittedAt,
                ApprovedAt = entity.ApprovedAt,
                LastRejectComment = entity.LastRejectComment,
                LastRejectAt = entity.LastRejectAt,
                Execution = entity.PaymentExecution != null ? new PaymentExecutionDataContract
                {
                    Id = entity.PaymentExecution.Id,
                    DataPagamento = entity.PaymentExecution.DataPagamento,
                    ContaBancariaFk = entity.PaymentExecution.ContaBancariaFk,
                    ContaBancariaNome = entity.PaymentExecution.ContaBancariaFkNavigation != null
                        ? $"{entity.PaymentExecution.ContaBancariaFkNavigation.EntidadeBancaria} ({entity.PaymentExecution.ContaBancariaFkNavigation.Numero})"
                        : null,
                    NumeroDocumento = entity.PaymentExecution.NumeroDocumento,
                    Observacao = entity.PaymentExecution.Observacao,
                    ExecutedAt = entity.PaymentExecution.ExecutedAt
                } : null
            };
        }

        public PaymentAuthorizationListResponse GetByAno(GetPaymentListRequest request)
        {
            PaymentAuthorizationListResponse response = new PaymentAuthorizationListResponse();
            try
            {
                response.Items = _unitOfWork.PaymentAuthorizationRepository.GetByAno(request.Ano)
                    .Select(MapEntity)
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ObligacoesDisponiveisParaPagamentoResponse GetObligacoesDisponiveis(GetObligacoesDisponiveisParaPagamentoRequest request)
        {
            ObligacoesDisponiveisParaPagamentoResponse response = new ObligacoesDisponiveisParaPagamentoResponse();
            try
            {
                List<Obligation> approved = _unitOfWork.ObligationRepository.GetByAno(request.Ano)
                    .Where(o => o.Estado == "APPROVED")
                    .ToList();

                response.Items = approved
                    .Where(o => !_unitOfWork.PaymentAuthorizationRepository.HasAuthorizationForObligation(o.Id))
                    .Select(o => new ObligacaoDisponivelParaPagamentoDataContract
                    {
                        ObligationId = o.Id,
                        Numero = o.Numero,
                        DescritivoObrigacao = o.DescritivoObrigacao,
                        ValorObrigacao = ValorObrigacao(o)
                    })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public CodigoContaOptionsResponse GetCodigoContaOptions()
        {
            CodigoContaOptionsResponse response = new CodigoContaOptionsResponse();
            try
            {
                response.Items = _unitOfWork.CodigoContaRepository.GetAllActiveCodigoConta()
                    .Select(c => new CodigoContaOptionDataContract { Id = c.Id, Designacao = c.Designacao })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ContaBancariaOptionsResponse GetContaBancariaOptions()
        {
            ContaBancariaOptionsResponse response = new ContaBancariaOptionsResponse();
            try
            {
                response.Items = _unitOfWork.ContaBancariaRepository.GetAllDto(false)
                    .Select(c => new ContaBancariaOptionDataContract { Id = c.Id, EntidadeBancaria = c.EntidadeBancaria, Numero = c.Numero })
                    .ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public PaymentAuthorizationResponse Create(CreatePaymentAuthorizationRequest request)
        {
            PaymentAuthorizationResponse response = new PaymentAuthorizationResponse { RequestId = request.RequestId };
            try
            {
                Obligation obligation = _unitOfWork.ObligationRepository.Get(request.ObligationFk);
                if (obligation == null || obligation.Estado != "APPROVED")
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-OBR-NOT-APPROVED", ErrorMessage = "Obrigação phải ở trạng thái đã duyệt trước khi tạo Autorização de Pagamento." });
                    return response;
                }

                if (_unitOfWork.PaymentAuthorizationRepository.HasAuthorizationForObligation(request.ObligationFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-DUP-OBR", ErrorMessage = "Obrigação này đã có Autorização de Pagamento." });
                    return response;
                }

                if (request.ValorAutorizado <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-INVALID-VALOR", ErrorMessage = "Giá trị phải lớn hơn 0." });
                    return response;
                }

                int numero = _unitOfWork.PaymentAuthorizationRepository.GetNextNumero(request.Mes, request.Ano);

                PaymentAuthorization entity = new PaymentAuthorization
                {
                    Numero = numero,
                    Mes = request.Mes,
                    Ano = request.Ano,
                    ObligationFk = request.ObligationFk,
                    Descritivo = request.Descritivo,
                    ValorAutorizado = request.ValorAutorizado,
                    CodigoContaDebitoFk = request.CodigoContaDebitoFk,
                    CodigoContaCreditoFk = request.CodigoContaCreditoFk,
                    Estado = ESTADO_DRAFT,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.PaymentAuthorizationRepository.Add(entity);
                _unitOfWork.Commit();

                PaymentAuthorization created = _unitOfWork.PaymentAuthorizationRepository.Get(entity.Id);
                response.Item = MapEntity(created);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Submit(SubmitPaymentAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                PaymentAuthorization entity = _unitOfWork.PaymentAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-NOT-FOUND", ErrorMessage = "Không tìm thấy Autorização de Pagamento." });
                    return response;
                }
                if (entity.Estado != ESTADO_DRAFT)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-NOT-DRAFT", ErrorMessage = "Autorização de Pagamento không ở trạng thái nháp." });
                    return response;
                }

                entity.Estado = ESTADO_PENDING_APPROVAL;
                entity.SubmittedBy = request.UserId;
                entity.SubmittedAt = DateTime.Now;
                entity = _utils.UpdateDetailsToEntity(entity);
                _unitOfWork.PaymentAuthorizationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Approve(ApprovePaymentAuthorizationRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                PaymentAuthorization entity = _unitOfWork.PaymentAuthorizationRepository.Get(request.Id);
                if (entity == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-NOT-FOUND", ErrorMessage = "Không tìm thấy Autorização de Pagamento." });
                    return response;
                }
                if (entity.Estado != ESTADO_PENDING_APPROVAL)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-WRONG-STATE", ErrorMessage = "Autorização de Pagamento không ở trạng thái chờ phê duyệt." });
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
                _unitOfWork.PaymentAuthorizationRepository.Update(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract Execute(ExecutePaymentRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                PaymentAuthorization authorization = _unitOfWork.PaymentAuthorizationRepository.Get(request.PaymentAuthorizationFk);
                if (authorization == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-NOT-FOUND", ErrorMessage = "Không tìm thấy Autorização de Pagamento." });
                    return response;
                }
                if (authorization.Estado != ESTADO_APPROVED)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-NOT-APPROVED", ErrorMessage = "Autorização de Pagamento chưa được duyệt." });
                    return response;
                }
                if (_unitOfWork.PaymentExecutionRepository.HasExecutionForAuthorization(request.PaymentAuthorizationFk))
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-ALREADY-EXECUTED", ErrorMessage = "Autorização de Pagamento này đã được thực hiện chi trả." });
                    return response;
                }

                PaymentExecution entity = new PaymentExecution
                {
                    PaymentAuthorizationFk = request.PaymentAuthorizationFk,
                    DataPagamento = request.DataPagamento,
                    ContaBancariaFk = request.ContaBancariaFk,
                    NumeroDocumento = request.NumeroDocumento,
                    Observacao = request.Observacao,
                    ExecutedBy = request.UserId,
                    ExecutedAt = DateTime.Now,
                    IndActivo = true
                };
                entity = _utils.SetDetailsToEntity(entity);
                _unitOfWork.PaymentExecutionRepository.Add(entity);
                _unitOfWork.Commit(); // entity.Id (identity) chỉ có giá trị thật sau Commit

                // Bút toán Débito/Crédito tự sinh ngay khi Pagamento thực hiện — dùng
                // đúng 2 tài khoản đã lưu ở PaymentAuthorization (bước duyệt), không nhập
                // tay riêng (xem memory lancamentos-conciliacao-link-design).
                _lancamentoDataManager.GerarSeChuaCo(
                    origemTipo: "PaymentExecution",
                    origemId: entity.Id,
                    data: request.DataPagamento,
                    codigoContaDebitoFk: authorization.CodigoContaDebitoFk,
                    codigoContaCreditoFk: authorization.CodigoContaCreditoFk,
                    valor: authorization.ValorAutorizado,
                    descricao: $"Pagamento Autorização Nº {authorization.Numero}/{authorization.Ano} - {authorization.Descritivo}");

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

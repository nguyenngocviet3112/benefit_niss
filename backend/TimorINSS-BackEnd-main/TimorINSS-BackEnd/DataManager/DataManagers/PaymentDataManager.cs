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

        // Tài khoản Phải trả trung gian dùng cho bút toán kép (xem Approve/Execute) —
        // xác nhận từ sổ sách thật (FRSSVF.xlsm "Lançamentos"): mọi Despesa có 2 bút
        // toán qua 1 tài khoản Phải trả (Fornecedores c/c/Com o pessoal/Outros credores...),
        // không phải Nợ Despesa / Có Ngân hàng trực tiếp trong 1 bút toán. Tài khoản phụ
        // thuộc BeneficiarioCategoria — trả về null nếu chưa cấu hình (soft-fail, giống
        // GuiaPagamentoContaConfig: GerarSeChuaCo tự bỏ qua nếu thiếu tài khoản).
        private int? GetContaPhaiTraFk(Obligation obligation)
        {
            return _unitOfWork.LiquidacaoContaConfigRepository.GetByCategoria(obligation?.BeneficiarioCategoria)?.CodigoContaFk;
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
                ObligationMes = entity.ObligationFkNavigation?.Mes ?? 0,
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
                } : null,
                LiquidacaoFaltaConfiguracao = entity.ApprovedAt.HasValue
                    && !_unitOfWork.LancamentoRepository.ExistsForOrigem("PaymentAuthorizationLiquidacao", entity.Id),
                ExecucaoFaltaConfiguracao = entity.PaymentExecution != null
                    && !_unitOfWork.LancamentoRepository.ExistsForOrigem("PaymentExecution", entity.PaymentExecution.Id)
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
                        Mes = o.Mes,
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

                decimal valorObrigacao = ValorObrigacao(obligation);
                if (request.ValorAutorizado > valorObrigacao)
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-EXCEEDS-OBR", ErrorMessage = $"Valor Autorizado vượt quá giá trị Obrigação ({valorObrigacao:N2})." });
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
                var attachmentConfig = _unitOfWork.AttachmentConfigRepository.Get();
                if (attachmentConfig.PagamentoObrigatorio && !_unitOfWork.AttachmentRepository.ExistsForEntity("PAGAMENTO", entity.Id))
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-ATTACHMENT-REQUIRED", ErrorMessage = "Bắt buộc đính kèm file trước khi submit Pagamento." });
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

                if (request.Approve)
                {
                    // Bút toán 1 (ghi nhận khoản Phải trả — "Liquidação") — sinh ngay khi
                    // Autorização de Pagamento được duyệt, TRƯỚC khi tiền thực rời ngân
                    // hàng. Nợ tài khoản Chi phí (đã chọn lúc tạo Autorização) / Có tài
                    // khoản Phải trả (theo BeneficiarioCategoria của Obrigação gốc). Xem
                    // memory liquidacao-conta-config-double-booking.
                    var lancResult = _lancamentoDataManager.GerarSeChuaCo(
                        origemTipo: "PaymentAuthorizationLiquidacao",
                        origemId: entity.Id,
                        data: entity.ApprovedAt ?? DateTime.Now,
                        codigoContaDebitoFk: entity.CodigoContaDebitoFk,
                        codigoContaCreditoFk: GetContaPhaiTraFk(entity.ObligationFkNavigation),
                        valor: entity.ValorAutorizado,
                        descricao: $"Autorização de Pagamento Nº {entity.Numero}/{entity.Ano} - {entity.Descritivo} (Liquidação)");

                    if (lancResult.FaltaConfiguracao)
                    {
                        response.Warnings.Add($"Đã duyệt, nhưng chưa ghi được bút toán Liquidação cho Autorização Nº {entity.Numero}/{entity.Ano} vì thiếu Tài khoản Nợ hoặc Tài khoản Phải trả — bổ sung ngay tại màn này (nút Ghi bù bút toán) để hoàn thiện sổ sách.");
                    }
                    else if (lancResult.Gerado)
                    {
                        response.Warnings.Add($"Đã tự động ghi bút toán Liquidação cho Autorização Nº {entity.Numero}/{entity.Ano}. Kiểm tra tại Registo de Lançamentos nếu cần điều chỉnh.");
                    }

                    _unitOfWork.Commit();
                }
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

                // Bút toán 2 (tất toán khoản Phải trả — tiền THỰC SỰ rời ngân hàng) — Nợ
                // tài khoản Phải trả (cùng tài khoản đã dùng ở bút toán 1 lúc duyệt
                // Autorização) / Có tài khoản Ngân hàng (đã chọn lúc tạo Autorização).
                // Không còn Nợ thẳng vào tài khoản Chi phí ở bước này nữa — đã chuyển
                // sang bút toán 1 tại Approve(), đúng theo sổ sách thật (xem memory
                // liquidacao-conta-config-double-booking).
                var lancResult = _lancamentoDataManager.GerarSeChuaCo(
                    origemTipo: "PaymentExecution",
                    origemId: entity.Id,
                    data: request.DataPagamento,
                    codigoContaDebitoFk: GetContaPhaiTraFk(authorization.ObligationFkNavigation),
                    codigoContaCreditoFk: authorization.CodigoContaCreditoFk,
                    valor: authorization.ValorAutorizado,
                    descricao: $"Pagamento Autorização Nº {authorization.Numero}/{authorization.Ano} - {authorization.Descritivo}");

                if (lancResult.FaltaConfiguracao)
                {
                    response.Warnings.Add($"Đã ghi nhận thực hiện chi trả, nhưng chưa ghi được bút toán tất toán cho Autorização Nº {authorization.Numero}/{authorization.Ano} vì thiếu Tài khoản Phải trả hoặc Tài khoản Ngân hàng — bổ sung ngay tại màn này (nút Ghi bù bút toán) để hoàn thiện sổ sách.");
                }
                else if (lancResult.Gerado)
                {
                    response.Warnings.Add($"Đã tự động ghi bút toán tất toán cho Autorização Nº {authorization.Numero}/{authorization.Ano}. Kiểm tra tại Registo de Lançamentos nếu cần điều chỉnh.");
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        // Ghi bù 1 bút toán đã bị bỏ qua vì thiếu Tài khoản Nợ/Có (FaltaConfiguracao),
        // cho phép người dùng nhập trực tiếp cặp tài khoản ngay tại màn Pagamento thay
        // vì phải đi cấu hình rồi không có cách nào quay lại ghi bù (Approve/Execute
        // chỉ chạy được đúng 1 lần). Giá trị/ngày/mô tả luôn lấy lại từ dữ liệu gốc,
        // không tin theo giá trị client gửi lên — chỉ 2 tài khoản là do người dùng chọn.
        public ResponseBaseDataContract CompletarLancamento(CompletarLancamentoPagamentoRequest request)
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

                // Bút toán 1 (Liquidação) và bút toán 2 (Execução) dùng CHUNG 1 tài khoản
                // Phải trả — Có ở bút toán 1 phải khớp Nợ ở bút toán 2 để triệt tiêu đúng
                // (xem comment GetContaPhaiTraFk). Nếu bút toán còn lại đã tồn tại, LUÔN
                // lấy tài khoản Phải trả từ đó, không tin theo lựa chọn của người dùng ở
                // phía này nữa — tránh 2 bên chọn lệch nhau làm sổ sách sai (2026-07-13,
                // phát hiện sau khi build tính năng Ghi bù bút toán).
                LancamentoGerarResult lancResult;
                bool contaPhaiTraOverridden = false;
                if (request.OrigemTipo == "PaymentAuthorizationLiquidacao")
                {
                    if (!authorization.ApprovedAt.HasValue)
                    {
                        response.Errors.Add(new Error { ErrorCode = "PAG-NOT-APPROVED", ErrorMessage = "Autorização de Pagamento chưa được duyệt." });
                        return response;
                    }

                    int contaPhaiTraFk = request.CodigoContaCreditoFk;
                    PaymentExecution existingExecution = _unitOfWork.PaymentExecutionRepository.GetByAuthorization(authorization.Id);
                    Lancamento execLancamento = existingExecution != null
                        ? _unitOfWork.LancamentoRepository.GetByOrigem("PaymentExecution", existingExecution.Id)
                        : null;
                    if (execLancamento != null && execLancamento.CodigoContaDebitoFk != request.CodigoContaCreditoFk)
                    {
                        contaPhaiTraFk = execLancamento.CodigoContaDebitoFk;
                        contaPhaiTraOverridden = true;
                    }

                    lancResult = _lancamentoDataManager.GerarSeChuaCo(
                        origemTipo: "PaymentAuthorizationLiquidacao",
                        origemId: authorization.Id,
                        data: authorization.ApprovedAt.Value,
                        codigoContaDebitoFk: request.CodigoContaDebitoFk,
                        codigoContaCreditoFk: contaPhaiTraFk,
                        valor: authorization.ValorAutorizado,
                        descricao: $"Autorização de Pagamento Nº {authorization.Numero}/{authorization.Ano} - {authorization.Descritivo} (Liquidação)");
                }
                else if (request.OrigemTipo == "PaymentExecution")
                {
                    PaymentExecution execution = _unitOfWork.PaymentExecutionRepository.GetByAuthorization(request.PaymentAuthorizationFk);
                    if (execution == null)
                    {
                        response.Errors.Add(new Error { ErrorCode = "PAG-NOT-EXECUTED", ErrorMessage = "Autorização de Pagamento này chưa được thực hiện chi trả." });
                        return response;
                    }

                    int contaPhaiTraFk = request.CodigoContaDebitoFk;
                    Lancamento liquidacaoLancamento = _unitOfWork.LancamentoRepository.GetByOrigem("PaymentAuthorizationLiquidacao", authorization.Id);
                    if (liquidacaoLancamento != null && liquidacaoLancamento.CodigoContaCreditoFk != request.CodigoContaDebitoFk)
                    {
                        contaPhaiTraFk = liquidacaoLancamento.CodigoContaCreditoFk;
                        contaPhaiTraOverridden = true;
                    }

                    lancResult = _lancamentoDataManager.GerarSeChuaCo(
                        origemTipo: "PaymentExecution",
                        origemId: execution.Id,
                        data: execution.DataPagamento,
                        codigoContaDebitoFk: contaPhaiTraFk,
                        codigoContaCreditoFk: request.CodigoContaCreditoFk,
                        valor: authorization.ValorAutorizado,
                        descricao: $"Pagamento Autorização Nº {authorization.Numero}/{authorization.Ano} - {authorization.Descritivo}");
                }
                else
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-INVALID-ORIGEM", ErrorMessage = "Loại bút toán không hợp lệ." });
                    return response;
                }

                if (contaPhaiTraOverridden)
                {
                    response.Warnings.Add("Tài khoản Phải trả bạn chọn không khớp với bút toán còn lại — đã tự động dùng đúng tài khoản Phải trả từ bút toán kia để sổ sách khớp nhau.");
                }

                if (lancResult.Gerado)
                {
                    _unitOfWork.Commit();
                }
                else
                {
                    response.Errors.Add(new Error { ErrorCode = "PAG-LANCAMENTO-ALREADY-EXISTS", ErrorMessage = "Bút toán này đã được ghi rồi (không cần bổ sung nữa)." });
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

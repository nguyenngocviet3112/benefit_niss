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
        private readonly ILancamentoDataManager _lancamentoDataManager;

        public BankStatementLineDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, ILancamentoDataManager lancamentoDataManager)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _lancamentoDataManager = lancamentoDataManager;
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
                DataTransacao = entity.DataTransacao,
                CodigoTransacaoBancaria = entity.CodigoTransacaoBancaria,
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
                response.Items = _unitOfWork.BankStatementLineRepository.GetByContaBancaria(request.ContaBancariaFk, request.DataInicio, request.DataFim)
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
                        ValorPac = r.ValorPac,
                        ValorCobradoBanco = r.ValorCobradoBanco
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
                    DataTransacao = request.DataTransacao,
                    CodigoTransacaoBancaria = request.CodigoTransacaoBancaria,
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

                ReceitaPac receita = _unitOfWork.ReceitaPacRepository.Get(request.ReceitaPacFk);
                if (receita == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "REC-NOT-FOUND", ErrorMessage = "Không tìm thấy Receita." });
                    return response;
                }
                // Chặn hẳn việc đối chiếu nếu Receita chưa cấu hình Tài khoản Nợ/Có,
                // thay vì cho đối chiếu xong rồi mới cảnh báo thiếu bút toán (2026-07-13,
                // user yêu cầu — phải cấu hình xong mới cho đối chiếu, tránh để đối chiếu
                // "treo" ở trạng thái thiếu sổ sách). ErrorCode riêng để frontend hiện
                // link thẳng tới màn Receita PAC.
                if (receita.CodigoContaDebitoFk == null || receita.CodigoContaCreditoFk == null)
                {
                    response.Errors.Add(new Error
                    {
                        ErrorCode = "REC-MISSING-CONTA-CONFIG",
                        ErrorMessage = $"Receita PAC Nº {receita.Numero}/{receita.Ano} chưa cấu hình Tài khoản Nợ/Có — vào màn Receita PAC để bổ sung 2 trường Tài khoản trước khi đối chiếu."
                    });
                    return response;
                }
                if (entity.Credito <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-NOT-CREDITO", ErrorMessage = "Dòng sao kê này không phải khoản Có (tiền vào) — không thể đối chiếu với Receita." });
                    return response;
                }
                if (receita.ValorCobradoBanco <= 0)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-RECEITA-NO-VALOR-BANCO", ErrorMessage = "Receita này chưa khai giá trị thu qua ngân hàng." });
                    return response;
                }
                if (entity.Credito != receita.ValorCobradoBanco)
                {
                    response.Errors.Add(new Error { ErrorCode = "BSL-AMOUNT-MISMATCH", ErrorMessage = "Giá trị dòng sao kê không khớp với giá trị thu qua ngân hàng đã khai báo trên Receita." });
                    return response;
                }

                entity.ReceitaPacFk = request.ReceitaPacFk;
                entity.ConciliadoBy = request.UserId;
                entity.ConciliadoAt = DateTime.Now;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);

                // Bút toán Débito/Crédito tự sinh ngay khi đối chiếu ngân hàng thành
                // công — đây là bước "tiền đã thực sự về" nên đúng chỗ để ghi sổ, thay
                // vì ghi ngay lúc Receita được nhập (số tự khai, chưa kiểm chứng) — xem
                // memory guia-pagamento-lancamento-wiring (cùng nguyên tắc áp dụng cho
                // Guia Pagamento trước đó). Tài khoản Nợ/Có đã được đảm bảo tồn tại ở
                // bước chặn phía trên nên GerarSeChuaCo ở đây luôn ghi được (không còn
                // nhánh FaltaConfiguracao khả dĩ nữa).
                var lancResult = _lancamentoDataManager.GerarSeChuaCo(
                    origemTipo: "ReceitaPacBanco",
                    origemId: receita.Id,
                    data: entity.DataValor,
                    codigoContaDebitoFk: receita.CodigoContaDebitoFk,
                    codigoContaCreditoFk: receita.CodigoContaCreditoFk,
                    valor: entity.Credito,
                    descricao: $"Receita PAC Nº {receita.Numero}/{receita.Ano} - {receita.Descritivo} (đối chiếu ngân hàng)");

                if (lancResult.Gerado)
                {
                    response.Warnings.Add($"Đã tự động ghi bút toán kế toán cho phần Banco của Receita Nº {receita.Numero}/{receita.Ano}. Kiểm tra tại Registo de Lançamentos nếu cần điều chỉnh.");
                }

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

                int? receitaPacFk = entity.ReceitaPacFk;

                entity.ReceitaPacFk = null;
                entity.PaymentExecutionFk = null;
                entity.ConciliadoBy = null;
                entity.ConciliadoAt = null;
                entity = (BankStatementLine)_utils.UpdateDetailsToEntity(entity);
                _unitOfWork.BankStatementLineRepository.Update(entity);

                // Hủy đối chiếu Receita thì bút toán đã sinh từ lần đối chiếu đó không
                // còn đúng nữa — tắt đi để lần đối chiếu kế tiếp (khớp dòng sao kê khác)
                // sinh lại bút toán mới đúng số liệu. PaymentExecution không cần xử lý
                // tương tự vì bút toán bên đó sinh ngay lúc Thực hiện chi trả (không
                // gắn với bước đối chiếu ở đây).
                if (receitaPacFk.HasValue)
                {
                    _lancamentoDataManager.DesfazerSeExiste("ReceitaPacBanco", receitaPacFk.Value);
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

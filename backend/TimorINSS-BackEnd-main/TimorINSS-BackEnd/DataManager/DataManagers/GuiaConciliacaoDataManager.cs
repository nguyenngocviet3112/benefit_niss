using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    // New-mode replacement for the "Duyệt" step of GuiaPagamentoDataManager.approveComprovativoPagamento:
    // that legacy method promotes a Guia straight to Paid/Partial Paid based only on manual
    // document review (officer typing in the amount they see on the uploaded evidence), with
    // no check against real bank data. This DataManager requires the officer to pick actual
    // BankStatementLine row(s) whose value matches the entidade's self-reported amount before
    // the same promotion happens, gated by the new-mode [RequirePerm] RBAC.
    // 2026-07-13: migrated off Movimentosbancarios/REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS (old
    // mode) onto BankStatementLine (already used by Conciliação de Movimentos for Receita/
    // Pagamento) + a new BankStatementLineGuiaPagamento junction table — user asked new-mode
    // to fully stop depending on old-mode's bank-statement infra ahead of eventually retiring
    // old-mode. Guia Pagamento genuinely needs N:N matching (client confirmed: 1 bank transfer
    // can cover several Guias, or 1 Guia can be paid across several transfers), which is why
    // this uses a real junction table rather than reusing BankStatementLine's simpler 1:1
    // ReceitaPacFk/PaymentExecutionFk columns.
    public class GuiaConciliacaoDataManager : IGuiaConciliacaoDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;
        private readonly ILancamentoDataManager _lancamentoDataManager;

        public GuiaConciliacaoDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils, ILancamentoDataManager lancamentoDataManager)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
            _lancamentoDataManager = lancamentoDataManager;
        }

        // Sổ sách thật (SCFSSTL2024_VF.xlsm "Lançamentos") dùng 2 tài khoản Crédito
        // khác nhau tùy khu vực (Setor) của Entidade đóng góp: "2133114 Setor Privado
        // - Guias Emitidas" (đa số) và "2133112 Setor Público - Guias Emitidas"
        // (thiểu số, có thật). Phân loại dựa vào Sectoractividade.Descricao — mọi
        // dòng seed hiện có (2026-07-13) đều đặt tên bắt đầu bằng "Setor Público"
        // cho các nhánh chính phủ (Governo/FFDTL/PNTL/Municípios/...), còn lại
        // ("Setor Privado", "Parceria Público-Privada (PPP)", hoặc thiếu dữ liệu)
        // coi là Privado — khớp đa số thực tế, không có ví dụ PPP riêng trong sổ.
        private static bool IsSetorPublico(Guiapagamento guia)
        {
            var descricao = guia?.GuiaEntidadeFkNavigation?.EntidadeSectorActFkNavigation?.Descricao;
            return descricao != null && descricao.StartsWith("Setor Público", StringComparison.OrdinalIgnoreCase);
        }

        // Dòng sao kê ngân hàng khả dụng để khớp Guia Pagamento — chưa khớp Receita/
        // Pagamento (BankStatementLine.ReceitaPacFk/PaymentExecutionFk) và chưa khớp
        // Guia nào khác (BankStatementLineGuiaPagamento). Chỉ trả Credito > 0 vì Guia
        // Pagamento luôn là tiền VÀO (giữ đúng hành vi cũ, trước đây lọc phía frontend).
        public BankStatementLineListResponse GetLinhasDisponiveis(SearchFilterRequest request)
        {
            var response = new BankStatementLineListResponse();
            try
            {
                var lines = _unitOfWork.BankStatementLineGuiaPagamentoRepository
                    .GetDisponiveisParaGuiaPagamento(request.filter?.dateFilterBegin, request.filter?.dateFilterEnd)
                    .Where(l => l.Credito > 0)
                    .ToList();

                response.Items = lines.Select(l => new DataContracts.ModelDataContract.BankStatementLineDataContract
                {
                    Id = l.Id,
                    ContaBancariaFk = l.ContaBancariaFk,
                    ContaBancariaNome = l.ContaBancariaFkNavigation != null
                        ? $"{l.ContaBancariaFkNavigation.EntidadeBancaria} ({l.ContaBancariaFkNavigation.Numero})"
                        : null,
                    EntidadeBancaria = l.ContaBancariaFkNavigation?.EntidadeBancaria,
                    DataValor = l.DataValor,
                    DataTransacao = l.DataTransacao,
                    CodigoTransacaoBancaria = l.CodigoTransacaoBancaria,
                    Descricao = l.Descricao,
                    Credito = l.Credito,
                    Debito = l.Debito,
                    IsConciliado = false
                }).ToList();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        public ResponseBaseDataContract ConciliarGuiaPagamento(ConciliarGuiaPagamentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            if (request.GuiaIds == null || request.GuiaIds.Count == 0 || request.BankStatementLineIds == null || request.BankStatementLineIds.Count == 0)
            {
                response.Errors.Add(new Error { ErrorCode = "GCONC-EMPTY-SELECTION", ErrorMessage = "Chọn ít nhất 1 Guia và 1 dòng sao kê ngân hàng." });
                return response;
            }

            var pagoStates = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
            var pendingStateIds = new[]
            {
                pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo em Validação")?.id,
                pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo Parcial em Validação")?.id,
            };

            var guiasPagamento = _unitOfWork.GuiaPagamentoRepository.GetGuiasByIds(request.GuiaIds);

            if (guiasPagamento.Count != request.GuiaIds.Count || guiasPagamento.Any(g => !pendingStateIds.Contains(g.IndPago)))
            {
                response.Errors.Add(new Error { ErrorCode = "GCONC-GUIA-NOT-PENDING", ErrorMessage = "Guia không tồn tại hoặc không ở trạng thái chờ xác nhận (Comprovativo em Validação)." });
                return response;
            }

            var movimentosAConciliar = request.GuiaIds.Select(id => new MovimentosAConciliar { Id = id, Type = MovimentosPorConciliarListagemType.GuiaPagamento }).ToList();

            var bankStatementLineValores = _unitOfWork.BankStatementLineRepository.GetValores(request.BankStatementLineIds);
            var guiasValores = _unitOfWork.MovimentosPorConciliarRepository.GetValores(movimentosAConciliar);

            var formIsValid = ((request.BankStatementLineIds.Count == 1 && request.GuiaIds.Count >= 1) ||
                               (request.GuiaIds.Count == 1 && request.BankStatementLineIds.Count >= 1)) &&
                               bankStatementLineValores.Count == request.BankStatementLineIds.Count &&
                               guiasValores.Count == request.GuiaIds.Count &&
                               bankStatementLineValores.Select(Math.Abs).Sum() == guiasValores.Select(Math.Abs).Sum() &&
                               !_unitOfWork.BankStatementLineGuiaPagamentoRepository.AnyGuiaAlreadyMatched(request.GuiaIds) &&
                               !_unitOfWork.BankStatementLineGuiaPagamentoRepository.AnyLineAlreadyMatched(request.BankStatementLineIds);

            if (!formIsValid)
            {
                response.Errors.Add(new Error { ErrorCode = "GCONC-AMOUNT-MISMATCH", ErrorMessage = "Tổng giá trị các dòng sao kê ngân hàng không khớp với số tiền Guia đã khai báo, hoặc đã được đối chiếu trước đó." });
                return response;
            }

            foreach (var lineId in request.BankStatementLineIds)
            {
                foreach (var guiaId in request.GuiaIds)
                {
                    var rel = new BankStatementLineGuiaPagamento
                    {
                        BankStatementLineFk = lineId,
                        GuiaPagamentoFk = guiaId,
                        IndActivo = true
                    };
                    rel = (BankStatementLineGuiaPagamento)_utils.SetDetailsToEntity(rel);
                    _unitOfWork.BankStatementLineGuiaPagamentoRepository.AddRelation(rel);
                }
            }

            var contasCorrente = new List<int>();
            foreach (var guia in guiasPagamento)
            {
                guia.IndPago = guia.IndPago == pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo em Validação")?.id
                    ? (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Paga").id
                    : (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Parcialmente Paga").id;

                if (!contasCorrente.Contains(guia.ContaCorrenteId)) contasCorrente.Add(guia.ContaCorrenteId);

                _utils.UpdateDetailsToEntity(guia);
                _unitOfWork.GuiaPagamentoRepository.Update(guia);
            }

            // Bút toán Débito/Crédito tự sinh ngay khi đối chiếu ngân hàng thành công —
            // đây là bước "tiền đã thực sự về" nên đúng chỗ để ghi sổ (giống PaymentExecution
            // bên Chi), khác với ReceitaPac (ghi sổ ngay khi nhập tay, chưa kiểm chứng ngân
            // hàng). Débito luôn là TÀI KHOẢN NGÂN HÀNG THẬT đã nhận tiền (tra qua
            // ContaFkNavigation.CodigoContaFk của dòng sao kê đã khớp) — xác nhận qua sổ
            // sách thật 2024 (SCFSSTL2024_VF.xlsm "Lançamentos": mọi bút toán thu GP Nợ
            // đúng ngân hàng cụ thể, không phải 1 tài khoản cố định — 2026-07-13, sửa lại
            // sau khi đối chiếu, xem memory guia-pagamento-lancamento-wiring). Crédito lấy
            // từ cấu hình admin (GuiaPagamentoContaConfig) NHƯNG khác nhau theo Setor
            // (Público/Privado) của Entidade đóng góp của TỪNG Guia — không còn 1 giá trị
            // chung nữa (2026-07-13, sửa sau khi user chỉ ra có nhiều loại công ty khác
            // nhau — xác nhận đúng qua sổ sách thật, xem memory
            // gp-setor-publico-privado-credito-split). Thiếu bất kỳ phần nào (dòng sao kê
            // ghi qua Caixa không phải Conta, ContaBancaria chưa map Codigoconta, hoặc
            // Crédito của Setor tương ứng chưa cấu hình) — bỏ qua lặng lẽ, không chặn việc
            // đối chiếu (GerarSeChuaCo tự bỏ qua khi thiếu debitoFk/creditoFk).
            var contaConfig = _unitOfWork.GuiaPagamentoContaConfigRepository.GetActive();
            var bankStatementLines = _unitOfWork.BankStatementLineRepository.GetByIds(request.BankStatementLineIds);

            // N dòng sao kê : 1 guia dùng ngân hàng của dòng sao kê đầu tiên — giả định
            // thông thường toàn bộ tiền của 1 Guia về cùng 1 ngân hàng; nếu về nhiều ngân
            // hàng khác nhau trong cùng 1 lần đối chiếu, đây là giới hạn đã biết (không xảy
            // ra ở chiều 1 dòng : N guias vì khi đó tất cả guias dùng chung dòng đó).
            var debitoContaBancariaFk = bankStatementLines.FirstOrDefault()?.ContaBancariaFkNavigation?.CodigoContaFk;

            foreach (var guia in guiasPagamento)
            {
                var creditoFk = IsSetorPublico(guia)
                    ? contaConfig?.CodigoContaCreditoPublicoFk
                    : contaConfig?.CodigoContaCreditoPrivadoFk;

                var lancResult = _lancamentoDataManager.GerarSeChuaCo(
                    origemTipo: "GuiaPagamento",
                    origemId: guia.IdGuia,
                    data: DateTime.Now,
                    codigoContaDebitoFk: debitoContaBancariaFk,
                    codigoContaCreditoFk: creditoFk,
                    valor: guia.ValorComprovPag ?? guia.Valor,
                    descricao: $"Guia Pagamento Nº {guia.NumDocumento} - đối chiếu ngân hàng");

                if (lancResult.FaltaConfiguracao)
                {
                    response.Warnings.Add($"Đối chiếu đã ghi nhận, nhưng chưa ghi được bút toán kế toán cho Guia {guia.NumDocumento} vì thiếu ánh xạ Tài khoản Ngân hàng (Cấu hình > Ánh xạ Tài khoản Ngân hàng) hoặc Tài khoản Có {(IsSetorPublico(guia) ? "Setor Público" : "Setor Privado")} (Cấu hình > Cấu hình Tài khoản Guia Pagamento) — bổ sung cấu hình rồi thử đối chiếu lại.");
                }
                else if (lancResult.Gerado)
                {
                    response.Warnings.Add($"Đã tự động ghi bút toán kế toán cho Guia {guia.NumDocumento}. Kiểm tra tại Registo de Lançamentos nếu cần điều chỉnh.");
                }
            }

            try
            {
                _unitOfWork.Commit();
                contasCorrente.ForEach(e => _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(e));
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }

            return response;
        }

        // Báo cáo "Receitas GP" (2026-07-14) — sổ đăng ký toàn bộ Guia Pagamento trong 1
        // năm, mọi entidade, mọi trạng thái kể cả đã đối chiếu/đã Paga — chỉ xem. Khác màn
        // Duyệt Guia Pagamento (GetLinhasDisponiveis/ConciliarGuiaPagamento), chỉ hiện Guia
        // đang chờ đối chiếu.
        public GuiaListagemResponse GetReceitasGpReport(GetReceitasGpReportRequest request)
        {
            var response = new GuiaListagemResponse { RequestId = request.RequestId };
            try
            {
                var result = _unitOfWork.GuiaPagamentoRepository.GetGuiasForReceitasGpReport(request.Ano);
                response.guias = result.guias;
                response.rows = result.rows;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }

        // Hủy 1 lần đối chiếu ngân hàng đã xác nhận sai (2026-07-14, user yêu cầu — cần có
        // đường lùi khi officer chọn nhầm dòng sao kê khớp với Guia). Đảo ngược đúng những gì
        // ConciliarGuiaPagamento đã làm: hạ IndPago về lại trạng thái chờ tương ứng, tắt
        // active quan hệ BankStatementLineGuiaPagamento (để dòng sao kê + Guia khả dụng lại
        // cho lần đối chiếu sau), và hủy Lançamento tự sinh (nếu có).
        public ResponseBaseDataContract UndoConciliacao(UndoConciliacaoGuiaRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };
            try
            {
                var guia = _unitOfWork.GuiaPagamentoRepository.GetGuiasByIds(new List<int> { request.GuiaId }).FirstOrDefault();
                if (guia == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "GCONC-GUIA-NOT-FOUND", ErrorMessage = "Không tìm thấy Guia." });
                    return response;
                }

                var pagoStates = _unitOfWork.DominioRepository.getAllTiposDeDominio(TiposDominio.INDPAGO);
                var guiaPagaId = (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Paga").id;
                var guiaParcialPagaId = (int)pagoStates.FirstOrDefault(x => x.descricao == "Guia Parcialmente Paga").id;
                var comprovativoValidacaoId = (int)pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo em Validação").id;
                var comprovativoParcialId = (int)pagoStates.FirstOrDefault(x => x.descricao == "Comprovativo Parcial em Validação").id;

                int? revertToId = null;
                if (guia.IndPago == guiaPagaId) revertToId = comprovativoValidacaoId;
                else if (guia.IndPago == guiaParcialPagaId) revertToId = comprovativoParcialId;

                if (revertToId == null)
                {
                    response.Errors.Add(new Error { ErrorCode = "GCONC-UNDO-NOT-RECONCILED", ErrorMessage = "Guia này chưa được đối chiếu ngân hàng — không có gì để hủy." });
                    return response;
                }

                guia.IndPago = revertToId.Value;
                _utils.UpdateDetailsToEntity(guia);
                _unitOfWork.GuiaPagamentoRepository.Update(guia);

                _unitOfWork.BankStatementLineGuiaPagamentoRepository.DeactivateForGuia(guia.IdGuia);
                _lancamentoDataManager.DesfazerSeExiste("GuiaPagamento", guia.IdGuia);

                _unitOfWork.Commit();
                _unitOfWork.ContaCorrenteRepository.UpdateSituacaoPagamento(guia.ContaCorrenteId, forceRecompute: true);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                _unitOfWork.Rollback();
            }
            return response;
        }

        // Chứng từ (comprovativo) + các số tự khai của 1 Guia đang chờ xác nhận — officer
        // xem để đối chiếu mắt thường với dòng sao kê ngân hàng thật trước khi chọn khớp
        // (2026-07-13, user yêu cầu: ngày upload/ngày trả tiền/ngân hàng giúp mapping).
        public GuiaComprovativoResponse GetComprovativo(GetGuiaComprovativoRequest request)
        {
            var response = new GuiaComprovativoResponse { RequestId = request.RequestId };

            var guia = _unitOfWork.GuiaPagamentoRepository.GetGuiasByIds(new List<int> { request.IdGuia }).FirstOrDefault();
            if (guia == null)
            {
                response.Errors.Add(new Error { ErrorCode = "GCONC-GUIA-NOT-FOUND", ErrorMessage = "Không tìm thấy Guia." });
                return response;
            }

            response.Guia = new GuiaComprovativoDataContract
            {
                IdGuia = guia.IdGuia,
                NumDocumento = guia.NumDocumento,
                DataUpload = guia.DataAlteracao,
                DataPagamento = guia.DataComprovPag,
                ValorPago = guia.ValorComprovPag,
                BankCode = guia.BankCode,
                ComprovativoPag = guia.ComprovativoPag
            };

            return response;
        }
    }
}

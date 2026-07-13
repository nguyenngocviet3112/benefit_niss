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
    // Movimentosbancarios line(s) whose value matches the entidade's self-reported amount
    // before the same promotion happens - mirrors the core matching logic already proven in
    // MovimentosPorConciliarDataManager.ConciliarMovimentos (GuiaPagamento branch), but gated
    // by the new-mode [RequirePerm] RBAC instead of the old Tarefa permission system.
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

        public ResponseBaseDataContract ConciliarGuiaPagamento(ConciliarGuiaPagamentoRequest request)
        {
            var response = new ResponseBaseDataContract { RequestId = request.RequestId };

            if (request.GuiaIds == null || request.GuiaIds.Count == 0 || request.MovimentosBancarios == null || request.MovimentosBancarios.Count == 0)
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

            var movimentosBancariosValores = _unitOfWork.MovimentosbancariosRepository.GetValores(request.MovimentosBancarios);
            var guiasValores = _unitOfWork.MovimentosPorConciliarRepository.GetValores(movimentosAConciliar);

            var formIsValid = ((request.MovimentosBancarios.Count == 1 && request.GuiaIds.Count >= 1) ||
                               (request.GuiaIds.Count == 1 && request.MovimentosBancarios.Count >= 1)) &&
                               movimentosBancariosValores.Count == request.MovimentosBancarios.Count &&
                               guiasValores.Count == request.GuiaIds.Count &&
                               movimentosBancariosValores.Select(Math.Abs).Sum() == guiasValores.Select(Math.Abs).Sum() &&
                               !_unitOfWork.MovimentosPorConciliarRepository.TemConciliados(movimentosAConciliar) &&
                               !_unitOfWork.MovimentosbancariosRepository.TemConciliados(request.MovimentosBancarios);

            if (!formIsValid)
            {
                response.Errors.Add(new Error { ErrorCode = "GCONC-AMOUNT-MISMATCH", ErrorMessage = "Tổng giá trị các dòng sao kê ngân hàng không khớp với số tiền Guia đã khai báo, hoặc đã được đối chiếu trước đó." });
                return response;
            }

            var estadoMovimento = _unitOfWork.DominioRepository.getIdDominio("ESTADOMOVIMENTO", 1);

            if (request.MovimentosBancarios.Count == 1)
            {
                var movBancarioId = request.MovimentosBancarios.First();
                movimentosAConciliar.ForEach(mov =>
                {
                    var rel = _unitOfWork.MovimentosPorConciliarRepository.CreateRelationObject(movBancarioId, mov, estadoMovimento);
                    _utils.SetDetailsToEntity(rel);
                    _unitOfWork.MovimentosPorConciliarRepository.AddRelation(rel);
                });
            }
            else
            {
                var guia = movimentosAConciliar.First();
                request.MovimentosBancarios.ForEach(mov =>
                {
                    var rel = _unitOfWork.MovimentosPorConciliarRepository.CreateRelationObject(mov, guia, estadoMovimento);
                    _utils.SetDetailsToEntity(rel);
                    _unitOfWork.MovimentosPorConciliarRepository.AddRelation(rel);
                });
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
            var movimentosBancarios = _unitOfWork.MovimentosbancariosRepository.GetByIds(request.MovimentosBancarios);

            // N movimentos : 1 guia dùng ngân hàng của dòng sao kê đầu tiên — giả định
            // thông thường toàn bộ tiền của 1 Guia về cùng 1 ngân hàng; nếu về nhiều ngân
            // hàng khác nhau trong cùng 1 lần đối chiếu, đây là giới hạn đã biết (không xảy
            // ra ở chiều 1 movimento : N guias vì khi đó tất cả guias dùng chung dòng đó).
            var debitoContaBancariaFk = movimentosBancarios.FirstOrDefault()?.ContaFkNavigation?.CodigoContaFk;

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

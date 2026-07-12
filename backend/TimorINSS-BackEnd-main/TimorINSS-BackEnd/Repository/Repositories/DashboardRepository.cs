using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // Trang tổng quan (Dashboard) — đếm nhanh số hồ sơ đang xử lý / đã duyệt cho
    // từng bước trong chu trình chi tiêu (AD -> Cabimento -> Compromisso ->
    // Obrigação -> Pagamento), cộng thêm số giao dịch ngân hàng đã/chưa đối
    // soát. Chỉ đếm (COUNT), không JOIN/Include gì thêm — dùng cho màn mặc
    // định khi đăng nhập nên cần nhẹ.
    public class DashboardRepository : IDashboardRepository
    {
        private const string ESTADO_APPROVED = "APPROVED";

        private readonly TimorINSSModuloContribuicoesContext _context;

        public DashboardRepository(TimorINSSModuloContribuicoesContext context)
        {
            _context = context;
        }

        public DashboardSummaryDataContract GetSummary()
        {
            return new DashboardSummaryDataContract
            {
                Ad = Summarize(_context.ExpenditureAuthorization.Where(e => e.IndActivo).Select(e => e.Estado)),
                Cabimento = Summarize(_context.Cabimento.Where(e => e.IndActivo).Select(e => e.Estado)),
                Compromisso = Summarize(_context.CompromissoDespesa.Where(e => e.IndActivo).Select(e => e.Estado)),
                Obrigacao = Summarize(_context.Obligation.Where(e => e.IndActivo).Select(e => e.Estado)),
                PagamentoAutorizacao = Summarize(_context.PaymentAuthorization.Where(e => e.IndActivo).Select(e => e.Estado)),
                PagamentoExecutado = _context.PaymentExecution.Count(e => e.IndActivo),
                BancoConciliado = _context.BankStatementLine.Count(e => e.IndActivo && e.ConciliadoAt != null),
                BancoPendente = _context.BankStatementLine.Count(e => e.IndActivo && e.ConciliadoAt == null),
            };
        }

        private static ProcessSummaryDataContract Summarize(IQueryable<string> estados)
        {
            var list = estados.ToList();
            int aprovado = list.Count(e => e == ESTADO_APPROVED);
            return new ProcessSummaryDataContract
            {
                Total = list.Count,
                Aprovado = aprovado,
                EmProcessamento = list.Count - aprovado
            };
        }
    }
}

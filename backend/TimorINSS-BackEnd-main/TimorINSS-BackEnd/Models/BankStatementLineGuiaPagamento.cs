using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    // Bảng nối N-N giữa BankStatementLine (sao kê ngân hàng mode mới) và
    // Guiapagamento (mode cũ) — thay thế REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS
    // chỉ cho nhánh Guia Pagamento, giữ đúng khả năng "1 dòng : N Guia" hoặc
    // "N dòng : 1 Guia" mà nghiệp vụ thực tế cần (khách xác nhận 2026-07-13).
    public partial class BankStatementLineGuiaPagamento
    {
        public int Id { get; set; }
        public int BankStatementLineFk { get; set; }
        public int GuiaPagamentoFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual BankStatementLine BankStatementLineFkNavigation { get; set; }
        public virtual Guiapagamento GuiaPagamentoFkNavigation { get; set; }
    }
}

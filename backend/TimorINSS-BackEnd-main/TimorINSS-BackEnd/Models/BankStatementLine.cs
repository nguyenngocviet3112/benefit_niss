using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class BankStatementLine
    {
        public int Id { get; set; }
        public int ContaBancariaFk { get; set; }
        public DateTime DataValor { get; set; }
        public DateTime? DataTransacao { get; set; }
        public string CodigoTransacaoBancaria { get; set; }
        public string Descricao { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public int? ReceitaPacFk { get; set; }
        public int? PaymentExecutionFk { get; set; }
        public int? ConciliadoBy { get; set; }
        public DateTime? ConciliadoAt { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Contabancaria ContaBancariaFkNavigation { get; set; }
        public virtual ReceitaPac ReceitaPacFkNavigation { get; set; }
        public virtual PaymentExecution PaymentExecutionFkNavigation { get; set; }
    }
}

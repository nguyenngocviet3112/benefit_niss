using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class OrcamentoSuplementarLinha
    {
        public int Id { get; set; }
        public int OrcamentoSuplementarFk { get; set; }
        public int OrcamentoLinhaFk { get; set; }
        public decimal OldValue { get; set; }
        public decimal AdjustmentValue { get; set; }
        public decimal FinalValue { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual OrcamentoSuplementar OrcamentoSuplementarFkNavigation { get; set; }
        public virtual OrcamentoLinha OrcamentoLinhaFkNavigation { get; set; }
    }
}

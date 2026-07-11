using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class OrcamentoLinha
    {
        public int Id { get; set; }
        public int OrcamentoBatchFk { get; set; }
        public int AtividadeFk { get; set; }
        public int EconomicClassificationFk { get; set; }
        public int? FunctionalClassificationFk { get; set; }
        public int OrganizationFk { get; set; }
        public decimal Valor { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual OrcamentoBatch OrcamentoBatchFkNavigation { get; set; }
        public virtual ProgramActivity AtividadeFkNavigation { get; set; }
        public virtual EconomicClassification EconomicClassificationFkNavigation { get; set; }
        public virtual FunctionalClassification FunctionalClassificationFkNavigation { get; set; }
        public virtual Institution OrganizationFkNavigation { get; set; }
    }
}

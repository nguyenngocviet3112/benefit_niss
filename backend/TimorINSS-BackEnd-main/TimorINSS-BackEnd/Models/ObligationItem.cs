using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ObligationItem
    {
        public int Id { get; set; }
        public int ObligationFk { get; set; }
        public int CompromissoDespesaFk { get; set; }
        public decimal Value { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Obligation ObligationFkNavigation { get; set; }
        public virtual CompromissoDespesa CompromissoDespesaFkNavigation { get; set; }
    }
}

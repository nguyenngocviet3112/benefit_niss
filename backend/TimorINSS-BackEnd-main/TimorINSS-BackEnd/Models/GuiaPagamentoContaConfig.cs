using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class GuiaPagamentoContaConfig
    {
        public int Id { get; set; }
        public int? CodigoContaCreditoPrivadoFk { get; set; }
        public int? CodigoContaCreditoPublicoFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Codigoconta CodigoContaCreditoPrivadoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaCreditoPublicoFkNavigation { get; set; }
    }
}

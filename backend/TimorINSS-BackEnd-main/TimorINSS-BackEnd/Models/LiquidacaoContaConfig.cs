using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class LiquidacaoContaConfig
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public int? CodigoContaFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Codigoconta CodigoContaFkNavigation { get; set; }
    }
}

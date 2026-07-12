using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Lancamento
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public int CodigoContaDebitoFk { get; set; }
        public int CodigoContaCreditoFk { get; set; }
        public decimal Valor { get; set; }
        public string OrigemTipo { get; set; }
        public int? OrigemId { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Codigoconta CodigoContaDebitoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaCreditoFkNavigation { get; set; }
    }
}

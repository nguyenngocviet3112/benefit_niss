using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class PaymentAuthorization
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int ObligationFk { get; set; }
        public string Descritivo { get; set; }
        public decimal ValorAutorizado { get; set; }
        public int? CodigoContaDebitoFk { get; set; }
        public int? CodigoContaCreditoFk { get; set; }
        public string Estado { get; set; }
        public int? SubmittedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string LastRejectComment { get; set; }
        public int? LastRejectBy { get; set; }
        public DateTime? LastRejectAt { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Obligation ObligationFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaDebitoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaCreditoFkNavigation { get; set; }
        public virtual PaymentExecution PaymentExecution { get; set; }
    }
}

using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class PaymentExecution
    {
        public int Id { get; set; }
        public int PaymentAuthorizationFk { get; set; }
        public DateTime DataPagamento { get; set; }
        public int? ContaBancariaFk { get; set; }
        public string NumeroDocumento { get; set; }
        public string Observacao { get; set; }
        public int ExecutedBy { get; set; }
        public DateTime ExecutedAt { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual PaymentAuthorization PaymentAuthorizationFkNavigation { get; set; }
        public virtual Contabancaria ContaBancariaFkNavigation { get; set; }
    }
}

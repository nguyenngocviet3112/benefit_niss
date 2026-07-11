using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Obligation
    {
        public Obligation()
        {
            ObligationItem = new HashSet<ObligationItem>();
            ObligationBeneficiary = new HashSet<ObligationBeneficiary>();
        }

        public int Id { get; set; }
        public int Numero { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string DescritivoObrigacao { get; set; }
        public string LiquidacaoTipo { get; set; }
        public string BeneficiarioNome { get; set; }
        public string BeneficiarioNiss { get; set; }
        public string BeneficiarioCategoria { get; set; }
        public string BeneficiarioNomeConta { get; set; }
        public string BeneficiarioNumeroConta { get; set; }
        public string BeneficiarioIban { get; set; }
        public string BeneficiarioSwift { get; set; }
        public string BeneficiarioBanco { get; set; }
        public decimal? BeneficiarioMontanteAPagar { get; set; }
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

        public virtual ICollection<ObligationItem> ObligationItem { get; set; }
        public virtual ICollection<ObligationBeneficiary> ObligationBeneficiary { get; set; }
    }
}

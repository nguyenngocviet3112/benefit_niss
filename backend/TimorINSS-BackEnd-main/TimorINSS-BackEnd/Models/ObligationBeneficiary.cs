using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ObligationBeneficiary
    {
        public int Id { get; set; }
        public int ObligationFk { get; set; }
        public string Niss { get; set; }
        public string NomeContribuinte { get; set; }
        public string NomeBeneficiario { get; set; }
        public string NomeConta { get; set; }
        public string NumeroConta { get; set; }
        public string Iban { get; set; }
        public string Swift { get; set; }
        public string Banco { get; set; }
        public decimal? SalarioIliquido { get; set; }
        public decimal? Cotizacao4 { get; set; }
        public decimal? Imposto10 { get; set; }
        public decimal? SalarioLiquido { get; set; }
        public decimal? OutrosSuplementos { get; set; }
        public decimal MontanteAPagar { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Obligation ObligationFkNavigation { get; set; }
    }
}

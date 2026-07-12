using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ReceitaPac
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Niss { get; set; }
        public int RegimeFk { get; set; }
        public int? AtividadeFk { get; set; }
        public int EconomicClassificationFk { get; set; }
        public int OrganizationFk { get; set; }
        public string Descritivo { get; set; }
        public decimal ValorPac { get; set; }
        public decimal ValorCobradoBanco { get; set; }
        public decimal ValorCobradoCaixa { get; set; }
        public int? ContaBancariaFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ProgramActivity RegimeFkNavigation { get; set; }
        public virtual ProgramActivity AtividadeFkNavigation { get; set; }
        public virtual EconomicClassification EconomicClassificationFkNavigation { get; set; }
        public virtual Institution OrganizationFkNavigation { get; set; }
        public virtual Contabancaria ContaBancariaFkNavigation { get; set; }
    }
}

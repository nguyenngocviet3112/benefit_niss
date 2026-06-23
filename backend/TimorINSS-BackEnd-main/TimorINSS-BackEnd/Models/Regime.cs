using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Regime
    {
        public Regime()
        {
            Declaracaoremuneracao = new HashSet<Declaracaoremuneracao>();
        }

        public int IdRegime { get; set; }
        public int TipoRegime { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal PercentEntidadeEmpreg { get; set; }
        public decimal PercentTrabalhador { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string NomeRegime { get; set; }
        public int RegimePai { get; set; }
        public int DataVencimento { get; set; }

        public virtual Dominio RegimePaiNavigation { get; set; }
        public virtual Dominio TipoRegimeNavigation { get; set; }
        public virtual ICollection<Declaracaoremuneracao> Declaracaoremuneracao { get; set; }
    }
}

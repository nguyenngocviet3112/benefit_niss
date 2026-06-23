using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Contacorrente
    {
        public Contacorrente()
        {
            Declaracaoremuneracao = new HashSet<Declaracaoremuneracao>();
            Guiapagamento = new HashSet<Guiapagamento>();
        }

        public int IdContaCorrente { get; set; }
        public int? ContaCorrenteEntidadeFk { get; set; }
        public int? ContaCorrenteTrabalhadorFk { get; set; }
        public int TipoDivida { get; set; }
        public DateTime MesAno { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorEntidade { get; set; }
        public decimal ValorTrabalhador { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime? PagoEm { get; set; }
        public int SituacaoPagamento { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? ContaCorrenteTaxaJuroFk { get; set; }
        public bool IndActivo { get; set; }
        public decimal? ValorJuros { get; set; }


        public virtual Entidadeempregadora ContaCorrenteEntidadeFkNavigation { get; set; }
        public virtual Taxajuromensal ContaCorrenteTaxaJuroFkNavigation { get; set; }
        public virtual Trabalhador ContaCorrenteTrabalhadorFkNavigation { get; set; }
        public virtual Dominio SituacaoPagamentoNavigation { get; set; }
        public virtual Dominio TipoDividaNavigation { get; set; }
        public virtual ICollection<Declaracaoremuneracao> Declaracaoremuneracao { get; set; }
        public virtual ICollection<Guiapagamento> Guiapagamento { get; set; }
    }
}

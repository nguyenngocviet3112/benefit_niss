using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Reservacredito
    {
        public Reservacredito()
        {
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
            RelMovimentosporconciliarMovimentos = new HashSet<RelMovimentosporconciliarMovimentos>();
        }

        public int IdReserva { get; set; }
        public int ReservaEntidadeFk { get; set; }
        public decimal? Valor { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? ReservaGuiaPagamentoFk { get; set; }

        public virtual Entidadeempregadora ReservaEntidadeFkNavigation { get; set; }
        public virtual Guiapagamento ReservaGuiaPagamentoFkNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
        public virtual ICollection<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
    }
}

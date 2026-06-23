using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class RelMovimentosporconciliarMovimentos
    {
        public RelMovimentosporconciliarMovimentos()
        {
            ComponentereceitaRegistoMovimentos = new HashSet<ComponentereceitaRegistoMovimentos>();
        }

        public int Id { get; set; }
        public int? MovimentoPorConciliarFk { get; set; }
        public int MovimentosBancariosFk { get; set; }
        public int? GuiaPagamentoFk { get; set; }
        public int? PagamentosExecutadosFk { get; set; }
        public DateTime DataCriacao { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public bool? IndActivo { get; set; }
        public string Ipv6 { get; set; }
        public int Estado { get; set; }
        public int? ReservaCreditoFk { get; set; }

        public virtual Guiapagamento GuiaPagamentoFkNavigation { get; set; }
        public virtual Movimentosporconciliar MovimentoPorConciliarFkNavigation { get; set; }
        public virtual Movimentosbancarios MovimentosBancariosFkNavigation { get; set; }
        public virtual Pagamentosexecutados PagamentosExecutadosFkNavigation { get; set; }
        public virtual Reservacredito ReservaCreditoFkNavigation { get; set; }
        public virtual ICollection<ComponentereceitaRegistoMovimentos> ComponentereceitaRegistoMovimentos { get; set; }
    }
}

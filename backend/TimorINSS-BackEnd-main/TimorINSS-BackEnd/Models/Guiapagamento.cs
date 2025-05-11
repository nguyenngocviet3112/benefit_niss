using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Guiapagamento
    {
        public Guiapagamento()
        {
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
            RelMovimentosporconciliarMovimentos = new HashSet<RelMovimentosporconciliarMovimentos>();
            Reservacredito = new HashSet<Reservacredito>();
        }

        public int IdGuia { get; set; }
        public int GuiaEntidadeFk { get; set; }
        public string NumDocumento { get; set; }
        public DateTime DtEmissao { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public int IndPago { get; set; }
        public byte[] ComprovativoPag { get; set; }
        public DateTime? DataComprovPag { get; set; }
        public decimal? ValorComprovPag { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public DateTime DtValidade { get; set; }

        public int TipoGuia { get; set; }
        public bool IndActivo { get; set; }
        public DateTime MesAno { get; set; }
        public int ContaCorrenteId { get; set; }
        public decimal? ValorJurosFixo { get; set; }
        public decimal? ValorJuros { get; set; }

        public string QrInvoice { get; set; }


        public string PaymentRef { get; set; }

        public string BankCode { get; set; }

        public virtual Contacorrente ContaCorrente { get; set; }
        public virtual Entidadeempregadora GuiaEntidadeFkNavigation { get; set; }
        public virtual Dominio IndPagoNavigation { get; set; }
        public virtual Dominio TipoGuiaNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
        public virtual ICollection<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
        public virtual ICollection<Reservacredito> Reservacredito { get; set; }
    }
}

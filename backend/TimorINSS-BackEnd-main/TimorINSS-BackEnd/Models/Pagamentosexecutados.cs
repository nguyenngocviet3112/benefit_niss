using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Pagamentosexecutados
    {
        public Pagamentosexecutados()
        {
            RelMovimentosporconciliarMovimentos = new HashSet<RelMovimentosporconciliarMovimentos>();
        }

        public int Id { get; set; }
        public int DestinatarioFk { get; set; }
        public string NumeroPagamento { get; set; }
        public int Estado { get; set; }
        public decimal ValorExecutado { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string Iban { get; set; }
        public int ProcessoAtivoFk { get; set; }
        public string NumeroConta { get; set; }
        public int? CodigoContaDebitoFk { get; set; }
        public int? CodigoContaCreditoFk { get; set; }
        public DateTime? DataObrigacao { get; set; }
        public int? DebitoExecucaoFk { get; set; }
        public int? CreditoExecucaoFk { get; set; }
        public DateTime? DataExecucao { get; set; }
        public int? CompromissoFk { get; set; }
        public string Swift { get; set; }

        public virtual Codigoconta CodigoContaCreditoFkNavigation { get; set; }
        public virtual Codigoconta CodigoContaDebitoFkNavigation { get; set; }
        public virtual Compromisso CompromissoFkNavigation { get; set; }
        public virtual Codigoconta CreditoExecucaoFkNavigation { get; set; }
        public virtual Codigoconta DebitoExecucaoFkNavigation { get; set; }
        public virtual Destinatario DestinatarioFkNavigation { get; set; }
        public virtual Dominio EstadoNavigation { get; set; }
        public virtual Processoativo ProcessoAtivoFkNavigation { get; set; }
        public virtual ICollection<RelMovimentosporconciliarMovimentos> RelMovimentosporconciliarMovimentos { get; set; }
    }
}

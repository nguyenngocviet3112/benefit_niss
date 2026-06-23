using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Codigoconta
    {
        public Codigoconta()
        {
            ComponentedespesaRegisto = new HashSet<ComponentedespesaRegisto>();
            ComponentereceitaRegistoCodigoContaDebitoFkNavigation = new HashSet<ComponentereceitaRegisto>();
            ComponentereceitaRegistoCodigoContaFkNavigation = new HashSet<ComponentereceitaRegisto>();
            InverseParentFkNavigation = new HashSet<Codigoconta>();
            MovimentosporconciliarCodigoContaCreditoFkNavigation = new HashSet<Movimentosporconciliar>();
            MovimentosporconciliarCodigoContaDebitoFkNavigation = new HashSet<Movimentosporconciliar>();
            PagamentosexecutadosCodigoContaCreditoFkNavigation = new HashSet<Pagamentosexecutados>();
            PagamentosexecutadosCodigoContaDebitoFkNavigation = new HashSet<Pagamentosexecutados>();
            PagamentosexecutadosCreditoExecucaoFkNavigation = new HashSet<Pagamentosexecutados>();
            PagamentosexecutadosDebitoExecucaoFkNavigation = new HashSet<Pagamentosexecutados>();
            Relcodigocontaagrupamentoconfig = new HashSet<Relcodigocontaagrupamentoconfig>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }
        public string Codigo { get; set; }
        public int OrcamentoConfigFk { get; set; }
        public int? ParentFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public decimal? InitialValue { get; set; }
        public DateTime? InitialValueDate { get; set; }
        public bool? IsCredit { get; set; }

        public virtual Orcamentoconfig OrcamentoConfigFkNavigation { get; set; }
        public virtual Codigoconta ParentFkNavigation { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegistoCodigoContaDebitoFkNavigation { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegistoCodigoContaFkNavigation { get; set; }
        public virtual ICollection<Codigoconta> InverseParentFkNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> MovimentosporconciliarCodigoContaCreditoFkNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> MovimentosporconciliarCodigoContaDebitoFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> PagamentosexecutadosCodigoContaCreditoFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> PagamentosexecutadosCodigoContaDebitoFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> PagamentosexecutadosCreditoExecucaoFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> PagamentosexecutadosDebitoExecucaoFkNavigation { get; set; }
        public virtual ICollection<Relcodigocontaagrupamentoconfig> Relcodigocontaagrupamentoconfig { get; set; }
    }
}

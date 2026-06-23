using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Agrupamentoconfig
    {
        public Agrupamentoconfig()
        {
            ComponentedespesaRegisto = new HashSet<ComponentedespesaRegisto>();
            Componenteorcamentovalor = new HashSet<Componenteorcamentovalor>();
            ComponentereceitaRegisto = new HashSet<ComponentereceitaRegisto>();
            InverseParentFkNavigation = new HashSet<Agrupamentoconfig>();
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
            Relcodigocontaagrupamentoconfig = new HashSet<Relcodigocontaagrupamentoconfig>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }
        public string Codigo { get; set; }
        public int ReltipoDeContaOrcamentoConfigFk { get; set; }
        public int? ParentFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Agrupamentoconfig ParentFkNavigation { get; set; }
        public virtual Reltipodecontaorcamentoconfig ReltipoDeContaOrcamentoConfigFkNavigation { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual ICollection<Componenteorcamentovalor> Componenteorcamentovalor { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual ICollection<Agrupamentoconfig> InverseParentFkNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
        public virtual ICollection<Relcodigocontaagrupamentoconfig> Relcodigocontaagrupamentoconfig { get; set; }
    }
}

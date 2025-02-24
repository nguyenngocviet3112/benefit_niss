using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Centrocusto
    {
        public Centrocusto()
        {
            ComponentedespesaRegisto = new HashSet<ComponentedespesaRegisto>();
            Componenteorcamentovalor = new HashSet<Componenteorcamentovalor>();
            ComponentereceitaRegisto = new HashSet<ComponentereceitaRegisto>();
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int OrcamentoconfigFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Orcamentoconfig OrcamentoconfigFkNavigation { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual ICollection<Componenteorcamentovalor> Componenteorcamentovalor { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
    }
}

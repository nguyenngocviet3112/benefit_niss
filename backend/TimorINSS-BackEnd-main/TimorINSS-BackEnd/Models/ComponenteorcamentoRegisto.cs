using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ComponenteorcamentoRegisto
    {
        public ComponenteorcamentoRegisto()
        {
            ComponentedespesaRegisto = new HashSet<ComponentedespesaRegisto>();
            Componenteorcamentovalor = new HashSet<Componenteorcamentovalor>();
            ComponentereceitaRegisto = new HashSet<ComponentereceitaRegisto>();
            InverseOrcamentoRetificadoFkNavigation = new HashSet<ComponenteorcamentoRegisto>();
        }

        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int TarefaActivoFk { get; set; }
        public bool Aprovado { get; set; }
        public int OrcamentoConfigFk { get; set; }
        public int? OrcamentoRetificadoFk { get; set; }

        public virtual Orcamentoconfig OrcamentoConfigFkNavigation { get; set; }
        public virtual ComponenteorcamentoRegisto OrcamentoRetificadoFkNavigation { get; set; }
        public virtual Tarefaativo TarefaActivoFkNavigation { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual ICollection<Componenteorcamentovalor> Componenteorcamentovalor { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual ICollection<ComponenteorcamentoRegisto> InverseOrcamentoRetificadoFkNavigation { get; set; }
    }
}

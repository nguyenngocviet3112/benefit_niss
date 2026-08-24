using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Tarefaativo
    {
        public Tarefaativo()
        {
            ComponenteaccoestarefaRegistoTarefaAtivoFkNavigation = new HashSet<ComponenteaccoestarefaRegisto>();
            ComponenteaccoestarefaRegistoTarefaSeguirFkNavigation = new HashSet<ComponenteaccoestarefaRegisto>();
            ComponenteclassificacaosubRegisto = new HashSet<ComponenteclassificacaosubRegisto>();
            ComponentedespesaRegisto = new HashSet<ComponentedespesaRegisto>();
            ComponentedocumentoRegisto = new HashSet<ComponentedocumentoRegisto>();
            ComponenteorcamentoRegisto = new HashSet<ComponenteorcamentoRegisto>();
            ComponentereceitaRegisto = new HashSet<ComponentereceitaRegisto>();
            ComponentetextoRegisto = new HashSet<ComponentetextoRegisto>();
            Compromisso = new HashSet<Compromisso>();
            Movimentosbancarios = new HashSet<Movimentosbancarios>();
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
        }

        public int Id { get; set; }
        public int ProcessoAtivoFk { get; set; }
        public int TarefaconfigFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? UtilizadorResponsavel { get; set; }
        public string TituloListaPagamento { get; set; }

        public virtual Processoativo ProcessoAtivoFkNavigation { get; set; }
        public virtual Tarefa TarefaconfigFkNavigation { get; set; }
        public virtual ICollection<ComponenteaccoestarefaRegisto> ComponenteaccoestarefaRegistoTarefaAtivoFkNavigation { get; set; }
        public virtual ICollection<ComponenteaccoestarefaRegisto> ComponenteaccoestarefaRegistoTarefaSeguirFkNavigation { get; set; }
        public virtual ICollection<ComponenteclassificacaosubRegisto> ComponenteclassificacaosubRegisto { get; set; }
        public virtual ICollection<ComponentedespesaRegisto> ComponentedespesaRegisto { get; set; }
        public virtual ICollection<ComponentedocumentoRegisto> ComponentedocumentoRegisto { get; set; }
        public virtual ICollection<ComponenteorcamentoRegisto> ComponenteorcamentoRegisto { get; set; }
        public virtual ICollection<ComponentereceitaRegisto> ComponentereceitaRegisto { get; set; }
        public virtual ICollection<ComponentetextoRegisto> ComponentetextoRegisto { get; set; }
        public virtual ICollection<Compromisso> Compromisso { get; set; }
        public virtual ICollection<Movimentosbancarios> Movimentosbancarios { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Tarefa
    {
        public Tarefa()
        {
            ComponenteaccoestarefaTarefaFkNavigation = new HashSet<Componenteaccoestarefa>();
            ComponenteaccoestarefaTarefaSeguirFkNavigation = new HashSet<Componenteaccoestarefa>();
            Componentecarregardocumento = new HashSet<Componentecarregardocumento>();
            Componenteclassificacaosubclassific = new HashSet<Componenteclassificacaosubclassific>();
            Componenteconciliacaomovimentos = new HashSet<Componenteconciliacaomovimentos>();
            Componentecontroleacesso = new HashSet<Componentecontroleacesso>();
            Componentedespesa = new HashSet<Componentedespesa>();
            Componenteorcamento = new HashSet<Componenteorcamento>();
            Componentereceita = new HashSet<Componentereceita>();
            Componentetexto = new HashSet<Componentetexto>();
            Relprocessoconfigtarefa = new HashSet<Relprocessoconfigtarefa>();
            Reltarefacomponente = new HashSet<Reltarefacomponente>();
            Tarefaativo = new HashSet<Tarefaativo>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int? PrazoTarefa { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool? HistoricoTexto { get; set; }
        public bool? HistoricoDocumento { get; set; }
        public bool? CabecalhoProcesso { get; set; }
        public string NumeroTarefa { get; set; }
        public bool BotaoArquivar { get; set; }

        public virtual ICollection<Componenteaccoestarefa> ComponenteaccoestarefaTarefaFkNavigation { get; set; }
        public virtual ICollection<Componenteaccoestarefa> ComponenteaccoestarefaTarefaSeguirFkNavigation { get; set; }
        public virtual ICollection<Componentecarregardocumento> Componentecarregardocumento { get; set; }
        public virtual ICollection<Componenteclassificacaosubclassific> Componenteclassificacaosubclassific { get; set; }
        public virtual ICollection<Componenteconciliacaomovimentos> Componenteconciliacaomovimentos { get; set; }
        public virtual ICollection<Componentecontroleacesso> Componentecontroleacesso { get; set; }
        public virtual ICollection<Componentedespesa> Componentedespesa { get; set; }
        public virtual ICollection<Componenteorcamento> Componenteorcamento { get; set; }
        public virtual ICollection<Componentereceita> Componentereceita { get; set; }
        public virtual ICollection<Componentetexto> Componentetexto { get; set; }
        public virtual ICollection<Relprocessoconfigtarefa> Relprocessoconfigtarefa { get; set; }
        public virtual ICollection<Reltarefacomponente> Reltarefacomponente { get; set; }
        public virtual ICollection<Tarefaativo> Tarefaativo { get; set; }
    }
}

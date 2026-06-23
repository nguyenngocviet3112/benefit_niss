using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Subclassificacao
    {
        public Subclassificacao()
        {
            ComponenteclassificacaosubRegisto = new HashSet<ComponenteclassificacaosubRegisto>();
            Componenteclassificacaosubclassific = new HashSet<Componenteclassificacaosubclassific>();
        }

        public int Id { get; set; }
        public int ClassificacaoFk { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Classificacao ClassificacaoFkNavigation { get; set; }
        public virtual ICollection<ComponenteclassificacaosubRegisto> ComponenteclassificacaosubRegisto { get; set; }
        public virtual ICollection<Componenteclassificacaosubclassific> Componenteclassificacaosubclassific { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Processoativo
    {
        public Processoativo()
        {
            Pagamentosexecutados = new HashSet<Pagamentosexecutados>();
            Tarefaativo = new HashSet<Tarefaativo>();
        }

        public int Id { get; set; }
        public int ProcessoConfigFk { get; set; }
        public bool Arquivado { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string NumeroProcesso { get; set; }

        public virtual Processoconfig ProcessoConfigFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> Pagamentosexecutados { get; set; }
        public virtual ICollection<Tarefaativo> Tarefaativo { get; set; }
    }
}

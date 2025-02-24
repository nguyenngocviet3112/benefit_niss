using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Compromisso
    {
        public Compromisso()
        {
            Pagamentosexecutados = new HashSet<Pagamentosexecutados>();
        }

        public int Id { get; set; }
        public int TarefaAtivoFk { get; set; }
        public int ComponenteDespesaRegistoFk { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool IndActivo { get; set; }

        public virtual ComponentedespesaRegisto ComponenteDespesaRegistoFkNavigation { get; set; }
        public virtual Tarefaativo TarefaAtivoFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> Pagamentosexecutados { get; set; }
    }
}

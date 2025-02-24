using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class ComponentetextoRegisto
    {
        public int Id { get; set; }
        public int TarefaAtivoFk { get; set; }
        public string TituloTexto { get; set; }
        public string Texto { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Tarefaativo TarefaAtivoFkNavigation { get; set; }
    }
}

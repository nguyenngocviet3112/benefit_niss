using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Actividadeeconomica
    {
        public Actividadeeconomica()
        {
            Entidadeempregadora = new HashSet<Entidadeempregadora>();
        }

        public int IdActivEconomica { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Entidadeempregadora> Entidadeempregadora { get; set; }
    }
}

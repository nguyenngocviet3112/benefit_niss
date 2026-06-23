using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Sectoractividade
    {
        public Sectoractividade()
        {
            Entidadeempregadora = new HashSet<Entidadeempregadora>();
        }

        public int IdSectorActividade { get; set; }
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

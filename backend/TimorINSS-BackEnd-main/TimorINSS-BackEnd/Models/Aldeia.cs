using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Aldeia
    {
        public Aldeia()
        {
            Morada = new HashSet<Morada>();
        }

        public int IdAldeia { get; set; }
        public int AldeiaSucoFk { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Suco AldeiaSucoFkNavigation { get; set; }
        public virtual ICollection<Morada> Morada { get; set; }
    }
}

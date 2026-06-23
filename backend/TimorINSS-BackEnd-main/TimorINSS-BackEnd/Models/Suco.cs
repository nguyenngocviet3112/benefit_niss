using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Suco
    {
        public Suco()
        {
            Aldeia = new HashSet<Aldeia>();
        }

        public int IdSuco { get; set; }
        public int SucoPostoAdminFk { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Postoadministrativo SucoPostoAdminFkNavigation { get; set; }
        public virtual ICollection<Aldeia> Aldeia { get; set; }
    }
}

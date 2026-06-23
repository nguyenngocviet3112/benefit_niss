using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Municipio
    {
        public Municipio()
        {
            Postoadministrativo = new HashSet<Postoadministrativo>();
        }

        public int IdMunicipio { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Postoadministrativo> Postoadministrativo { get; set; }
    }
}

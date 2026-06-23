using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Pais
    {
        public Pais()
        {
            Inssestrangeiro = new HashSet<Inssestrangeiro>();
            Morada = new HashSet<Morada>();
        }

        public int IdPais { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Indicativo { get; set; }
        public string Iso { get; set; }
        public string Iso3 { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Inssestrangeiro> Inssestrangeiro { get; set; }
        public virtual ICollection<Morada> Morada { get; set; }
    }
}

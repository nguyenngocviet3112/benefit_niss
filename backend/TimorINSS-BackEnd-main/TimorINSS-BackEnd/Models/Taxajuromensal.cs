using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Taxajuromensal
    {
        public Taxajuromensal()
        {
            Contacorrente = new HashSet<Contacorrente>();
        }

        public int IdTaxa { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool IndActivo { get; set; }
        public decimal Percentagem { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public virtual ICollection<Contacorrente> Contacorrente { get; set; }
    }
}

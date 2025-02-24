using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Orcamentoconfig
    {
        public Orcamentoconfig()
        {
            Centrocusto = new HashSet<Centrocusto>();
            Codigoconta = new HashSet<Codigoconta>();
            ComponenteorcamentoRegisto = new HashSet<ComponenteorcamentoRegisto>();
            Reltipodecontaorcamentoconfig = new HashSet<Reltipodecontaorcamentoconfig>();
        }

        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Centrocusto> Centrocusto { get; set; }
        public virtual ICollection<Codigoconta> Codigoconta { get; set; }
        public virtual ICollection<ComponenteorcamentoRegisto> ComponenteorcamentoRegisto { get; set; }
        public virtual ICollection<Reltipodecontaorcamentoconfig> Reltipodecontaorcamentoconfig { get; set; }
    }
}

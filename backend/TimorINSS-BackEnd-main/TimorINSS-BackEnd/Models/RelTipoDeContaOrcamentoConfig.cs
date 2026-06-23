using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Reltipodecontaorcamentoconfig
    {
        public Reltipodecontaorcamentoconfig()
        {
            Agrupamentoconfig = new HashSet<Agrupamentoconfig>();
        }

        public int Id { get; set; }
        public int OrcamentoConfigFk { get; set; }
        public int TipoContaFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Orcamentoconfig OrcamentoConfigFkNavigation { get; set; }
        public virtual Dominio TipoContaFkNavigation { get; set; }
        public virtual ICollection<Agrupamentoconfig> Agrupamentoconfig { get; set; }
    }
}

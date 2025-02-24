using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Camposeditaveis
    {
        public Camposeditaveis()
        {
            InverseCampoPaiFkNavigation = new HashSet<Camposeditaveis>();
        }

        public int IdCampoEditavel { get; set; }
        public string Nome { get; set; }
        public int DominioFk { get; set; }
        public string DominioString { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? CampoPaiFk { get; set; }

        public virtual Camposeditaveis CampoPaiFkNavigation { get; set; }
        public virtual Dominio DominioFkNavigation { get; set; }
        public virtual ICollection<Camposeditaveis> InverseCampoPaiFkNavigation { get; set; }
    }
}

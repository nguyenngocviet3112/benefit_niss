using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Movimentobancario
    {
        public Movimentobancario()
        {
            Movimentosporconciliar = new HashSet<Movimentosporconciliar>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public int TipoMovimento { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? CampoPaiFk { get; set; }

        public virtual Dominio TipoMovimentoNavigation { get; set; }
        public virtual ICollection<Movimentosporconciliar> Movimentosporconciliar { get; set; }
    }
}

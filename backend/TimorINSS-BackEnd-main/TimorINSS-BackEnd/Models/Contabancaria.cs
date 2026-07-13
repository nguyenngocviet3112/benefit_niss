using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Contabancaria
    {
        public Contabancaria()
        {
            Movimentosbancarios = new HashSet<Movimentosbancarios>();
        }

        public int Id { get; set; }
        public string Swift { get; set; }
        public string EntidadeBancaria { get; set; }
        public string Descricao { get; set; }
        public string Iban { get; set; }
        public string Numero { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? CampoPaiFk { get; set; }
        public int? CodigoContaFk { get; set; }

        public virtual Codigoconta CodigoContaFkNavigation { get; set; }
        public virtual ICollection<Movimentosbancarios> Movimentosbancarios { get; set; }
    }
}

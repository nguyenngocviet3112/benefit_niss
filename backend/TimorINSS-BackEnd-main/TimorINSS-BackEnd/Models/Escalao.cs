using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Escalao
    {
        public Escalao()
        {
            Relentidadetrabalhador = new HashSet<Relentidadetrabalhador>();
        }

        public int IdEscalao { get; set; }
        public string DescNivelEscalao { get; set; }
        public decimal Valor { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int EscalaoRegimeFk { get; set; }

        public virtual Dominio EscalaoRegimeFkNavigation { get; set; }
        public virtual ICollection<Relentidadetrabalhador> Relentidadetrabalhador { get; set; }
    }
}

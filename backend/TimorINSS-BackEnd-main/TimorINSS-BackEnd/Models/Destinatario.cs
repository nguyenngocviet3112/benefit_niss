using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Destinatario
    {
        public Destinatario()
        {
            Pagamentosexecutados = new HashSet<Pagamentosexecutados>();
        }

        public int Id { get; set; }
        public int? EntidadeFk { get; set; }
        public int? TrabalhadorFk { get; set; }
        public string Nome { get; set; }
        public string Niss { get; set; }
        public string Tin { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public string Morada { get; set; }

        public virtual Entidadeempregadora EntidadeFkNavigation { get; set; }
        public virtual Trabalhador TrabalhadorFkNavigation { get; set; }
        public virtual ICollection<Pagamentosexecutados> Pagamentosexecutados { get; set; }
    }
}

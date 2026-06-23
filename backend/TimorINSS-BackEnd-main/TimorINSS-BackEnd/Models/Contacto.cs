using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Contacto
    {
        public int IdContacto { get; set; }
        public int? ContactoTrabalhadorFk { get; set; }
        public int? ContactoEntidadeFk { get; set; }
        public string Telemovel { get; set; }
        public string Email { get; set; }
        public bool IndActivo { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Entidadeempregadora ContactoEntidadeFkNavigation { get; set; }
        public virtual Trabalhador ContactoTrabalhadorFkNavigation { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Utilizador
    {
        public Utilizador()
        {
            Componentecontroleacesso = new HashSet<Componentecontroleacesso>();
            Relutilizadordepartamento = new HashSet<Relutilizadordepartamento>();
            Relutilizadorperfil = new HashSet<Relutilizadorperfil>();
        }

        public int IdUtilizador { get; set; }
        public int? UtilizadorEntidadeFk { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public bool? Interno { get; set; }
        public int? TrabalhadorFk { get; set; }
        public int LoginAttempts { get; set; }
        public bool Locked { get; set; }

        public virtual Trabalhador TrabalhadorFkNavigation { get; set; }
        public virtual Entidadeempregadora UtilizadorEntidadeFkNavigation { get; set; }
        public virtual ICollection<Componentecontroleacesso> Componentecontroleacesso { get; set; }
        public virtual ICollection<Relutilizadordepartamento> Relutilizadordepartamento { get; set; }
        public virtual ICollection<Relutilizadorperfil> Relutilizadorperfil { get; set; }
    }
}

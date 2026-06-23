using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Perfil
    {
        public Perfil()
        {
            Componentecontroleacesso = new HashSet<Componentecontroleacesso>();
            Relperfilfuncionalidade = new HashSet<Relperfilfuncionalidade>();
            Relprocessoconfigperfil = new HashSet<Relprocessoconfigperfil>();
            Relutilizadorperfil = new HashSet<Relutilizadorperfil>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Componentecontroleacesso> Componentecontroleacesso { get; set; }
        public virtual ICollection<Relperfilfuncionalidade> Relperfilfuncionalidade { get; set; }
        public virtual ICollection<Relprocessoconfigperfil> Relprocessoconfigperfil { get; set; }
        public virtual ICollection<Relutilizadorperfil> Relutilizadorperfil { get; set; }
    }
}

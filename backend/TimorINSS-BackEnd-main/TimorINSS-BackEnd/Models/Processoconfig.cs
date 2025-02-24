using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Processoconfig
    {
        public Processoconfig()
        {
            Processoativo = new HashSet<Processoativo>();
            Relprocessoconfigperfil = new HashSet<Relprocessoconfigperfil>();
            Relprocessoconfigtarefa = new HashSet<Relprocessoconfigtarefa>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual ICollection<Processoativo> Processoativo { get; set; }
        public virtual ICollection<Relprocessoconfigperfil> Relprocessoconfigperfil { get; set; }
        public virtual ICollection<Relprocessoconfigtarefa> Relprocessoconfigtarefa { get; set; }
    }
}

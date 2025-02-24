using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relperfilfuncionalidade
    {
        public int Id { get; set; }
        public int PerfilFk { get; set; }
        public int FuncionalidadeFk { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Funcionalidade FuncionalidadeFkNavigation { get; set; }
        public virtual Perfil PerfilFkNavigation { get; set; }
    }
}

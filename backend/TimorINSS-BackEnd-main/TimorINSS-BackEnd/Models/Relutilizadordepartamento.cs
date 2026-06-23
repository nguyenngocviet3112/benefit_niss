using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relutilizadordepartamento
    {
        public int Id { get; set; }
        public int UtilizadorFk { get; set; }
        public int DepartamentoFk { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Departamento DepartamentoFkNavigation { get; set; }
        public virtual Utilizador UtilizadorFkNavigation { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Morada
    {
        public int IdMorada { get; set; }
        public int? MoradaAldeiaFk { get; set; }
        public string Rua { get; set; }
        public string NumPorta { get; set; }
        public int MoradaPaisFk { get; set; }
        public bool FlagImportado { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int? TrabalhadorMoradaFk { get; set; }
        public int? EntidadeMoradaFk { get; set; }
        public bool MoradaPrincipal { get; set; }

        public virtual Entidadeempregadora EntidadeMoradaFkNavigation { get; set; }
        public virtual Aldeia MoradaAldeiaFkNavigation { get; set; }
        public virtual Pais MoradaPaisFkNavigation { get; set; }
        public virtual Trabalhador TrabalhadorMoradaFkNavigation { get; set; }
    }
}

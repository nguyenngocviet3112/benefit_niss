using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relentidademorada
    {
        public int IdRel { get; set; }
        public int RelEntidadeFk { get; set; }
        public int RelMoradaFk { get; set; }
        public string IsMoradaPrincipal { get; set; }
        public string IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Entidadeempregadora RelEntidadeFkNavigation { get; set; }
        public virtual Morada RelMoradaFkNavigation { get; set; }
    }
}
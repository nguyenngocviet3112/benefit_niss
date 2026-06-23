using System;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relentidadesuspensao
    {
        public int IdRel { get; set; }
        public int EntidadeSuspensaoFk { get; set; }
        public DateTime DataInicioSuspensao { get; set; }
        public DateTime? DataFimSuspensao { get; set; }
        public string IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Entidadeempregadora EntidadeSuspensaoFkNavigation { get; set; }
    }
}
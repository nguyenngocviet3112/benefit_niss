using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Relentidaderesplegal
    {
        public int IdRel { get; set; }
        public int RelEntidadeRespFk { get; set; }
        public int RelRespLegalEntFk { get; set; }
        public DateTime DataInicioFuncao { get; set; }
        public DateTime? DataFimFuncao { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }

        public virtual Entidadeempregadora RelEntidadeRespFkNavigation { get; set; }
        public virtual Responsavellegal RelRespLegalEntFkNavigation { get; set; }
    }
}

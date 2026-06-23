using System;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelentidaderesplegalDto : BaseDto
    {
        [Mapper]
        public int IdRel { get; set; }

        [Mapper]
        public int RelEntidadeRespFk { get; set; }

        [Mapper]
        public int RelRespLegalEntFk { get; set; }

        [Mapper]
        public DateTime DataInicioFuncao { get; set; }

        [Mapper]
        public DateTime? DataFimFuncao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual EntidadeempregadoraDto Entidade { get; set; }
        public virtual ResponsavellegalDto Trabalhador { get; set; }
    }
}
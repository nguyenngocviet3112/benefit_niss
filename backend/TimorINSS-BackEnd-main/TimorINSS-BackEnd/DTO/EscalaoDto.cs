using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class EscalaoDto : BaseDto
    {
        [Mapper]
        public int IdEscalao { get; set; }

        [Mapper]
        public string DescNivelEscalao { get; set; }

        [Mapper]
        public decimal Valor { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int EscalaoRegimeFk { get; set; }

        public virtual DominioDto EscalaoRegimeFkNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> Relentidadetrabalhador { get; set; }
    }
}
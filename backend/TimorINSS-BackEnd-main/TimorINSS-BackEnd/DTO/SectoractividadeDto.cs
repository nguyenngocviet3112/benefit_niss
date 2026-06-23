using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class SectoractividadeDto : BaseDto
    {
        [Mapper]
        public int IdSectorActividade { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<EntidadeempregadoraDto> Entidadeempregadora { get; set; }
    }
}
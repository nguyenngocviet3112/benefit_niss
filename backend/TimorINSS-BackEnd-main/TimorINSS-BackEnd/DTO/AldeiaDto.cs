using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class AldeiaDto : BaseDto
    {
        [Mapper]
        public int IdAldeia { get; set; }

        [Mapper]
        public int AldeiaSucoFk { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual SucoDto AldeiaSucoFkNavigation { get; set; }
        public virtual ICollection<MoradaDto> Morada { get; set; }
    }
}
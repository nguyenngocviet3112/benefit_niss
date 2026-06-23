using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class MunicipioDto : BaseDto
    {
        [Mapper]
        public int IdMunicipio { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<PostoadministrativoDto> Postoadministrativo { get; set; }
    }
}
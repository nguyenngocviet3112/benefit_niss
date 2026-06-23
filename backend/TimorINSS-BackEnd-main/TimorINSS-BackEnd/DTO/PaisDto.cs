using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class PaisDto : BaseDto
    {
        [Mapper]
        public int IdPais { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public string? Codigo { get; set; }

        [Mapper]
        public string? Indicativo { get; set; }

        [Mapper]
        public string? Iso { get; set; }

        [Mapper]
        public string? Iso3 { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<InssestrangeiroDto> Inssestrangeiro { get; set; }
        public virtual ICollection<MoradaDto> Morada { get; set; }
    }
}
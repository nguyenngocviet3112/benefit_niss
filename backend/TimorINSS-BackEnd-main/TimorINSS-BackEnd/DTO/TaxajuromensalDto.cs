using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class TaxajuromensalDto : BaseDto
    {
        [Mapper]
        public int IdTaxa { get; set; }

        [Mapper]
        public decimal Percentagem { get; set; }

        [Mapper]
        public DateTime DataInicio { get; set; }

        [Mapper]
        public DateTime? DataFim { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<ContacorrenteDto> Contacorrente { get; set; }
    }
}
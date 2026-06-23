using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class DepartamentoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<ComponenteOrcamentoValorDto> Componenteorcamentovalor { get; set; }
        public virtual ICollection<RelUtilizadorDepartamentoDto> RelUtilizadorDeparatamento { get; set; }
    }
}
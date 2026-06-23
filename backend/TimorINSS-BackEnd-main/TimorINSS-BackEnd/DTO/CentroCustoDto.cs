using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class CentroCustoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Descricao { get; set; }

        [Mapper]
        public DateTime DataInicio { get; set; }

        [Mapper]
        public DateTime? DataFim { get; set; }

        [Mapper]
        public int OrcamentoconfigFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual OrcamentoConfigDto OrcamentoconfigFkNavigation { get; set; }
        public virtual ICollection<ComponenteOrcamentoValorDto> Componenteorcamentovalor { get; set; }
    }
}
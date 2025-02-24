using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class OrcamentoConfigDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public DateTime DataInicio { get; set; }

        [Mapper]
        public DateTime? DataFim { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<CentroCustoDto> Centrocusto { get; set; }
        public virtual ICollection<CodigoContaDto> Codigoconta { get; set; }
        public virtual ICollection<ComponenteOrcamentoRegistoDto> ComponenteorcamentoRegisto { get; set; }
        public virtual ICollection<RelTipoDeContaOrcamentoConfigDto> Reltipodecontaorcamentoconfig { get; set; }
    }
}
using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteOrcamentoRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaActivoFk { get; set; }

        [Mapper]
        public DateTime DataInicio { get; set; }

        [Mapper]
        public DateTime DataFim { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool Aprovado { get; set; }

        [Mapper]
        public int OrcamentoConfigFk { get; set; }

        [Mapper]
        public int? OrcamentoRetificadoFk { get; set; }

        public virtual OrcamentoConfigDto OrcamentoConfigFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaActivoFkNavigation { get; set; }
        public virtual ICollection<ComponenteOrcamentoValorDto> Componenteorcamentovalor { get; set; }
        public virtual ComponenteOrcamentoRegistoDto OrcamentoRetificadoFkNavigation { get; set; }
        public virtual ICollection<ComponenteOrcamentoRegistoDto> InverseOrcamentoRetificadoFkNavigation { get; set; }
    }
}
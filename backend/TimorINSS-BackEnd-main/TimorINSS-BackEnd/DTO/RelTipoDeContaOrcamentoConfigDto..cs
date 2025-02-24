using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelTipoDeContaOrcamentoConfigDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int OrcamentoConfigFk { get; set; }

        [Mapper]
        public int TipoContaFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual OrcamentoConfigDto OrcamentoConfigFkNavigation { get; set; }
        public virtual DominioDto TipoContaFkNavigation { get; set; }
        public virtual ICollection<AgrupamentoConfigDto> Agrupamentoconfig { get; set; }
    }
}
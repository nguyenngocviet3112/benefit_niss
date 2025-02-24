using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteReceitaRegistoMovimentosDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ComponenteReceitaRegistoId { get; set; }

        [Mapper]
        public int RelMovimentosPorConciliarMovimentosId { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }
    }
}
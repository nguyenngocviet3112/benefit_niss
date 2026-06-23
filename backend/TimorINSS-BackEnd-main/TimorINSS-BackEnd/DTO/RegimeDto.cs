using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RegimeDto : BaseDto
    {
        [Mapper]
        public int IdRegime { get; set; }

        [Mapper]
        public int TipoRegime { get; set; }

        [Mapper]
        public DateTime DataInicio { get; set; }

        [Mapper]
        public DateTime? DataFim { get; set; }

        [Mapper]
        public decimal PercentEntidadeEmpreg { get; set; }

        [Mapper]
        public decimal PercentTrabalhador { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int DataVencimento { get; set; }

        [Mapper]
        public int RegimePai { get; set; }

        [Mapper]
        public string NomeRegime { get; set; }

        public virtual DominioDto RegimePaiNavigation { get; set; }
        public virtual DominioDto TipoRegimeNavigation { get; set; }
        public virtual ICollection<RelentidadetrabalhadorDto> Relentidadetrabalhador { get; set; }
    }
}
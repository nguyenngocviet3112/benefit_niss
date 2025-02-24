using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class CodigoContaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Designacao { get; set; }

        [Mapper]
        public string Codigo { get; set; }

        [Mapper]
        public int OrcamentoConfigFk { get; set; }

        [Mapper]
        public int? ParentFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public decimal? InitialValue { get; set; }

        [Mapper]
        public DateTime? InitialValueDate { get; set; }

        [Mapper]
        public bool? IsCredit { get; set; }

        public virtual OrcamentoConfigDto OrcamentoConfigFkNavigation { get; set; }
        public virtual CodigoContaDto ParentFkNavigation { get; set; }
        public virtual ICollection<CodigoContaDto> InverseParentFkNavigation { get; set; }
        public virtual ICollection<RelCodigoContaAgrupamentoConfigDto> Relcodigocontaagrupamentoconfig { get; set; }
    }
}
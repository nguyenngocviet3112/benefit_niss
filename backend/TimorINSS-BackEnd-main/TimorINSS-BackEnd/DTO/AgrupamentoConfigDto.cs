using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class AgrupamentoConfigDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Designacao { get; set; }

        [Mapper]
        public string Codigo { get; set; }

        [Mapper]
        public int ReltipoDeContaOrcamentoConfigFk { get; set; }

        [Mapper]
        public int? ParentFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual AgrupamentoConfigDto ParentFkNavigation { get; set; }
        public virtual Reltipodecontaorcamentoconfig ReltipoDeContaOrcamentoConfigFkNavigation { get; set; }
        public virtual ICollection<AgrupamentoConfigDto> InverseParentFkNavigation { get; set; }
        public virtual ICollection<RelCodigoContaAgrupamentoConfigDto> Relcodigocontaagrupamentoconfig { get; set; }
        public virtual ICollection<ComponenteOrcamentoValorDto> Componenteorcamentovalor { get; set; }
    }
}
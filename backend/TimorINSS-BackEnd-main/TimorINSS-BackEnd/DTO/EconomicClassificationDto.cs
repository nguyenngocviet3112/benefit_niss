using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class EconomicClassificationDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Codigo { get; set; }

        [Mapper]
        public string Designacao { get; set; }

        [Mapper]
        public int Nivel { get; set; }

        [Mapper]
        public int? ParentFk { get; set; }

        [Mapper]
        public int OrcamentoConfigFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual EconomicClassificationDto ParentFkNavigation { get; set; }
        public virtual ICollection<EconomicClassificationDto> InverseParentFkNavigation { get; set; }
    }
}

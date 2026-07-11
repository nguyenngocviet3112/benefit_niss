using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ProgramActivityDto : BaseDto
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

        public virtual ProgramActivityDto ParentFkNavigation { get; set; }
        public virtual ICollection<ProgramActivityDto> InverseParentFkNavigation { get; set; }
    }
}

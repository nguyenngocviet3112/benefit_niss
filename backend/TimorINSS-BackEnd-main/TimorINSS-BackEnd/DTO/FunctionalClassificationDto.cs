using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class FunctionalClassificationDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Codigo { get; set; }

        [Mapper]
        public string Designacao { get; set; }

        [Mapper]
        public int? ParentFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual FunctionalClassificationDto ParentFkNavigation { get; set; }
        public virtual ICollection<FunctionalClassificationDto> InverseParentFkNavigation { get; set; }
    }
}

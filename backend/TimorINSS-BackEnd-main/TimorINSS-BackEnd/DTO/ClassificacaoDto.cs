using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ClassificacaoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ICollection<SubClassificacaoDto> Subclassificacao { get; set; }
    }
}
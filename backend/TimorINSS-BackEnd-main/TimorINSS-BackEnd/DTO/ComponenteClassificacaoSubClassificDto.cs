using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteClassificacaoSubClassificDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int SubclassificacaoFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual SubClassificacaoDto SubclassificacaoFkNavigation { get; set; }
        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
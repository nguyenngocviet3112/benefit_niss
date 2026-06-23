using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteclassificacaosubRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaAtivoFk { get; set; }

        [Mapper]
        public int SubClassificacaoFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual SubClassificacaoDto SubClassificacaoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
    }
}
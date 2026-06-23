using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteaccoestarefaRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaAtivoFk { get; set; }

        [Mapper]
        public int TarefaSeguirFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaSeguirFkNavigation { get; set; }
    }
}
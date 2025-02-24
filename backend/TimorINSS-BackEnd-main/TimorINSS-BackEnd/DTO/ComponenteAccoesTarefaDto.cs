using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteAccoesTarefaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int TarefaSeguirFk { get; set; }

        [Mapper]
        public string ApelidoDaTarefa { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class RelTarefaComponenteDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int ComponenteFk { get; set; }

        [Mapper]
        public int Ordem { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool Expandir { get; set; }

        public virtual ComponenteDto ComponenteFkNavigation { get; set; }
        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
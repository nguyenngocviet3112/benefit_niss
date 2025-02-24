using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class Relprocessoconfigtarefadto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ProcessoConfigFk { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public bool TarefaInicial { get; set; }

        [Mapper]
        public virtual ProcessoconfigDto ProcessoConfigFkNavigation { get; set; }

        [Mapper]
        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
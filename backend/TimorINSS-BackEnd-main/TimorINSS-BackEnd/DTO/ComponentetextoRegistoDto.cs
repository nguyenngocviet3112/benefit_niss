using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponentetextoRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaAtivoFk { get; set; }

        [Mapper]
        public string TituloTexto { get; set; }

        [Mapper]
        public string Texto { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
    }
}
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteCarregarDocumentoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int DocumentoFk { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public bool Obrigatorio { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual DominioDto DocumentoFkNavigation { get; set; }
        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
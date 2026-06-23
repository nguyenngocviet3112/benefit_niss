using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponentedocumentoRegistoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaAtivoFk { get; set; }

        [Mapper]
        public int DocumentoFk { get; set; }

        [Mapper]
        public byte[] Documento { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public string Numero { get; set; }

        public virtual DominioDto DocumentoFkNavigation { get; set; }
        public virtual TarefaativoDto TarefaAtivoFkNavigation { get; set; }
    }
}
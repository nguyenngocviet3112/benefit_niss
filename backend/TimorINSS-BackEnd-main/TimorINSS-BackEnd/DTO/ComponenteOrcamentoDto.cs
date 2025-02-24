using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteOrcamentoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int PermissaoDatas { get; set; }

        [Mapper]
        public int PermissaoInsercoes { get; set; }

        [Mapper]
        public int PermissaoDetalhes { get; set; }

        [Mapper]
        public int PermissaoAprovacao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
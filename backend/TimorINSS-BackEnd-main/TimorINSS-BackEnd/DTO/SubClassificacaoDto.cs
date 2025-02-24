using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class SubClassificacaoDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int ClassificacaoFk { get; set; }

        [Mapper]
        public string Nome { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual ClassificacaoDto ClassificacaoFkNavigation { get; set; }
    }
}
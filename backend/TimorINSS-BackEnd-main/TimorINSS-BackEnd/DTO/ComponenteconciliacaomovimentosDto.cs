using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteconciliacaomovimentosDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int PermissaoSelecionarMovimentos { get; set; }

        [Mapper]
        public int PermissaoMovimentosConciliar { get; set; }

        [Mapper]
        public int PermissaoMovimentosBancarios { get; set; }

        [Mapper]
        public int PermissaoVerMovimentosAconciliar { get; set; }

        [Mapper]
        public int PermissaoConciliar { get; set; }

        [Mapper]
        public int PermissaoDesfazerConciliar { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteReceitaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int ClassificarMovSelecionados { get; set; }

        [Mapper]
        public int SelecionarMovRecebidosParaRegisto { get; set; }

        [Mapper]
        public int VerificarExecucaoOrcamentoEditarSelecao { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
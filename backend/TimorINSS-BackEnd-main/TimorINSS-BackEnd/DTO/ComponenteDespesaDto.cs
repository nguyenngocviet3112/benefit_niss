using System.Runtime.Serialization;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DTO
{
    public class ComponenteDespesaDto : BaseDto
    {
        [Mapper]
        public int Id { get; set; }

        [Mapper]
        public int TarefaFk { get; set; }

        [Mapper]
        public int RegistarDespesa { get; set; }

        [Mapper]
        public int VisualizarDespesaR { get; set; }

        [Mapper]
        public int VisualizarDespesaA { get; set; }

        [Mapper]
        public int VisualizarDespesaComCompromisso { get; set; }

        [Mapper]
        public int VisualiazarDespesaC { get; set; }

        [Mapper]
        public int ExecutarPagamentos { get; set; }

        [Mapper]
        public int VisualizarExecucaoDespesaCabimentada { get; set; }

        [Mapper]
        public bool IndActivo { get; set; }

        [Mapper]
        public int VisualizarDespesaRparaA { get; set; }

        [Mapper]
        public int VisualizarDespesaAparaC { get; set; }

        [Mapper]
        public int EmitirOrdemPagamento { get; set; }

        public virtual TarefaDto TarefaFkNavigation { get; set; }
    }
}
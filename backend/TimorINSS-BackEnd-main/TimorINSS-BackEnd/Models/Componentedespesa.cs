using System;
using System.Collections.Generic;

#nullable disable

namespace TimorINSSBackEnd.Models
{
    public partial class Componentedespesa
    {
        public int Id { get; set; }
        public int TarefaFk { get; set; }
        public int RegistarDespesa { get; set; }
        public int VisualizarDespesaR { get; set; }
        public int VisualizarDespesaA { get; set; }
        public int VisualiazarDespesaC { get; set; }
        public int ExecutarPagamentos { get; set; }
        public int VisualizarExecucaoDespesaCabimentada { get; set; }
        public bool IndActivo { get; set; }
        public int UtilizadorCriacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public int? UtilizadorAlteracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Ipv6 { get; set; }
        public int VisualizarDespesaRparaA { get; set; }
        public int VisualizarDespesaAparaC { get; set; }
        public int EmitirOrdemPagamento { get; set; }
        public int VisualizarDespesaComCompromisso { get; set; }

        public virtual Tarefa TarefaFkNavigation { get; set; }
    }
}

using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // One row = one Classificação Económica code (Agrupamentoconfig), Receita (4xx) or Despesa (5xx).
    [DataContract]
    public class CeInssGlobalDataContract
    {
        [DataMember]
        public int agrupamentoId { get; set; }

        [DataMember]
        public string codigo { get; set; }

        [DataMember]
        public string designacao { get; set; }

        [DataMember]
        public decimal valorOrcamentoInicial { get; set; }

        [DataMember]
        public decimal valorOrcamentado { get; set; }

        // Despesa only
        [DataMember]
        public decimal cabimentos { get; set; }

        // Despesa only
        [DataMember]
        public decimal compromissos { get; set; }

        // Receita only — "Receita Liquidada"; Despesa side reuses this as "Total Execução" (paid)
        [DataMember]
        public decimal totalExecucao { get; set; }

        [DataMember]
        public decimal taxaExecucao { get; set; }

        [DataMember]
        public decimal saldoExecucao { get; set; }

        // Despesa only — Valor comprometido e não liquidado (compromissos - totalExecucao)
        [DataMember]
        public decimal saldoComprometidoNaoLiquidado { get; set; }

        // Despesa only — Valor cabimentado e não comprometido (cabimentos - compromissos)
        [DataMember]
        public decimal saldoCabimentadoNaoComprometido { get; set; }
    }
}

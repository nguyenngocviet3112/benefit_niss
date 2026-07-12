using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // Ciclo da Despesa — 1 row per AD (Autorização de Despesa), tracking the full execution
    // chain Cabimento -> Compromisso -> Obrigação -> Pagamento with running balances at each
    // stage. Mirrors the client's "Ciclo_Despesa" sheet (INSS_2026_janeiro _original.xlsx)
    // column-for-column: Regime, Atividade, Classificação Económica, Classificação Funcional,
    // N.º AD, Cabimentos, Compromissos, Saldo1, Obrigações, Saldo2, Pagamentos, Saldo3.
    [DataContract]
    public class CicloDespesaDataContract
    {
        [DataMember]
        public int adId { get; set; }

        [DataMember]
        public int numeroAd { get; set; }

        [DataMember]
        public string regimeCodigo { get; set; }

        [DataMember]
        public string regimeDesignacao { get; set; }

        [DataMember]
        public string atividadeCodigo { get; set; }

        [DataMember]
        public string atividadeDesignacao { get; set; }

        [DataMember]
        public string classificacaoEconomicaCodigo { get; set; }

        [DataMember]
        public string classificacaoEconomicaDesignacao { get; set; }

        [DataMember]
        public string classificacaoFuncionalCodigo { get; set; }

        [DataMember]
        public string classificacaoFuncionalDesignacao { get; set; }

        [DataMember]
        public decimal cabimentos { get; set; }

        [DataMember]
        public decimal compromissos { get; set; }

        // Saldo1 = Cabimentos - Compromissos
        [DataMember]
        public decimal saldo1 { get; set; }

        [DataMember]
        public decimal obrigacoes { get; set; }

        // Saldo2 = Compromissos - Obrigações
        [DataMember]
        public decimal saldo2 { get; set; }

        [DataMember]
        public decimal pagamentos { get; set; }

        // Saldo3 = Obrigações - Pagamentos
        [DataMember]
        public decimal saldo3 { get; set; }
    }
}

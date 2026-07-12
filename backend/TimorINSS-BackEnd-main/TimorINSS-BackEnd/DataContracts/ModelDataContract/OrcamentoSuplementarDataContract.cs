using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class OrcamentoSuplementarLinhaDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrcamentoLinhaFk { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string AtividadeDesignacao { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public string OrganizationNome { get; set; }

        [DataMember]
        public decimal OldValue { get; set; }

        [DataMember]
        public decimal AdjustmentValue { get; set; }

        [DataMember]
        public decimal FinalValue { get; set; }
    }

    [DataContract]
    public class OrcamentoSuplementarBatchDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public DateTime? SubmittedAt { get; set; }

        [DataMember]
        public DateTime? ReviewedAt { get; set; }

        [DataMember]
        public DateTime? ApprovedAt { get; set; }

        [DataMember]
        public string LastRejectComment { get; set; }

        [DataMember]
        public DateTime? LastRejectAt { get; set; }

        [DataMember]
        public decimal TotalAdjustment { get; set; }

        [DataMember]
        public List<OrcamentoSuplementarLinhaDataContract> Linhas { get; set; } = new List<OrcamentoSuplementarLinhaDataContract>();
    }

    [DataContract]
    public class RubricaAprovadaParaSuplementarDataContract
    {
        [DataMember]
        public int OrcamentoLinhaId { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string AtividadeDesignacao { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public string OrganizationNome { get; set; }

        [DataMember]
        public decimal ValorAtual { get; set; }
    }
}

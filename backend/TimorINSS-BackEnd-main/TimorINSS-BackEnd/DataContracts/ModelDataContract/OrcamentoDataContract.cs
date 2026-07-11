using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class OrcamentoLinhaDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int AtividadeFk { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string AtividadeDesignacao { get; set; }

        [DataMember]
        public int EconomicClassificationFk { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public int? FunctionalClassificationFk { get; set; }

        [DataMember]
        public string FunctionalClassificationCodigo { get; set; }

        [DataMember]
        public string FunctionalClassificationDesignacao { get; set; }

        [DataMember]
        public int OrganizationFk { get; set; }

        [DataMember]
        public string OrganizationNome { get; set; }

        [DataMember]
        public decimal Valor { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }
    }

    [DataContract]
    public class OrcamentoBatchDataContract
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
        public decimal TotalValor { get; set; }

        [DataMember]
        public List<OrcamentoLinhaDataContract> Linhas { get; set; } = new List<OrcamentoLinhaDataContract>();
    }
}

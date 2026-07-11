using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ExpenditureAuthorizationPlurianualidadeDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class ExpenditureAuthorizationDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }

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
        public decimal RubricaValor { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorAutorizado { get; set; }

        [DataMember]
        public decimal Regularizacao { get; set; }

        [DataMember]
        public decimal ValorRevisto { get; set; }

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
        public List<ExpenditureAuthorizationPlurianualidadeDataContract> Plurianualidade { get; set; } = new List<ExpenditureAuthorizationPlurianualidadeDataContract>();
    }

    [DataContract]
    public class RubricaDisponivelDataContract
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
        public decimal Valor { get; set; }
    }
}

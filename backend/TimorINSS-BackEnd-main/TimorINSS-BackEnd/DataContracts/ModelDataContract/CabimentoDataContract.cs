using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CabimentoDataContract
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
        public int ExpenditureAuthorizationFk { get; set; }

        [DataMember]
        public int ExpenditureAuthorizationNumero { get; set; }

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
        public decimal ValorAutorizadoAd { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorCabimentado { get; set; }

        [DataMember]
        public decimal ValorComprometido { get; set; }

        [DataMember]
        public decimal SaldoDisponivel { get; set; }

        [DataMember]
        public bool? ProcessoAprovisionamentoPrevio { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public DateTime? SubmittedAt { get; set; }

        [DataMember]
        public DateTime? ApprovedAt { get; set; }

        [DataMember]
        public string LastRejectComment { get; set; }

        [DataMember]
        public DateTime? LastRejectAt { get; set; }
    }

    [DataContract]
    public class AdDisponivelParaCabimentoDataContract
    {
        [DataMember]
        public int ExpenditureAuthorizationId { get; set; }

        [DataMember]
        public int Numero { get; set; }

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
        public decimal ValorRevisto { get; set; }
    }
}

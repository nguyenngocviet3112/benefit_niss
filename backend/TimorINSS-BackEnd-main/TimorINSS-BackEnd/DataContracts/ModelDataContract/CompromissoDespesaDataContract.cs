using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CompromissoDespesaPlurianualidadeDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class CompromissoDespesaDataContract
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
        public int CabimentoFk { get; set; }

        [DataMember]
        public int CabimentoNumero { get; set; }

        [DataMember]
        public int CabimentoMes { get; set; }

        [DataMember]
        public int ExpenditureAuthorizationNumero { get; set; }

        [DataMember]
        public int ExpenditureAuthorizationMes { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string AtividadeDesignacao { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public string FunctionalClassificationCodigo { get; set; }

        [DataMember]
        public string FunctionalClassificationDesignacao { get; set; }

        [DataMember]
        public string OrganizationNome { get; set; }

        [DataMember]
        public decimal ValorCabimentado { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorCompromissoGlobal { get; set; }

        [DataMember]
        public decimal ValorCompromissoAno { get; set; }

        [DataMember]
        public decimal Regularizacao { get; set; }

        [DataMember]
        public decimal ValorRevisto { get; set; }

        [DataMember]
        public decimal ValorObrigado { get; set; }

        [DataMember]
        public decimal SaldoDisponivel { get; set; }

        [DataMember]
        public string AssumidoCom { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public DateTime? SubmittedAt { get; set; }

        [DataMember]
        public DateTime? ReviewedAt { get; set; }

        [DataMember]
        public string ReviewComment { get; set; }

        [DataMember]
        public DateTime? ApprovedAt { get; set; }

        [DataMember]
        public string ApproveComment { get; set; }

        [DataMember]
        public string LastRejectComment { get; set; }

        [DataMember]
        public DateTime? LastRejectAt { get; set; }

        [DataMember]
        public List<CompromissoDespesaPlurianualidadeDataContract> Plurianualidade { get; set; } = new List<CompromissoDespesaPlurianualidadeDataContract>();
    }

    [DataContract]
    public class CabimentoDisponivelParaCompromissoDataContract
    {
        [DataMember]
        public int CabimentoId { get; set; }

        [DataMember]
        public int Numero { get; set; }

        [DataMember]
        public int Mes { get; set; }

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
        public decimal ValorCabimentado { get; set; }
    }
}

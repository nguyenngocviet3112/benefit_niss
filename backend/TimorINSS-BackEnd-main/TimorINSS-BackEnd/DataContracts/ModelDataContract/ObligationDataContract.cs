using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ObligationItemDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int CompromissoDespesaFk { get; set; }

        [DataMember]
        public int CompromissoDespesaNumero { get; set; }

        [DataMember]
        public string AtividadeCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationCodigo { get; set; }

        [DataMember]
        public string EconomicClassificationDesignacao { get; set; }

        [DataMember]
        public decimal CompromissoValorRevisto { get; set; }

        [DataMember]
        public decimal CompromissoSaldoDisponivel { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class ObligationBeneficiaryDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public string NomeContribuinte { get; set; }

        [DataMember]
        public string NomeBeneficiario { get; set; }

        [DataMember]
        public string NomeConta { get; set; }

        [DataMember]
        public string NumeroConta { get; set; }

        [DataMember]
        public string Iban { get; set; }

        [DataMember]
        public string Swift { get; set; }

        [DataMember]
        public string Banco { get; set; }

        [DataMember]
        public decimal? SalarioIliquido { get; set; }

        [DataMember]
        public decimal? Cotizacao4 { get; set; }

        [DataMember]
        public decimal? Imposto10 { get; set; }

        [DataMember]
        public decimal? SalarioLiquido { get; set; }

        [DataMember]
        public decimal? OutrosSuplementos { get; set; }

        [DataMember]
        public decimal MontanteAPagar { get; set; }
    }

    [DataContract]
    public class ObligationDataContract
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
        public string DescritivoObrigacao { get; set; }

        [DataMember]
        public decimal ValorObrigacao { get; set; }

        [DataMember]
        public string LiquidacaoTipo { get; set; }

        [DataMember]
        public string BeneficiarioNome { get; set; }

        [DataMember]
        public string BeneficiarioNiss { get; set; }

        [DataMember]
        public string BeneficiarioCategoria { get; set; }

        [DataMember]
        public string BeneficiarioNomeConta { get; set; }

        [DataMember]
        public string BeneficiarioNumeroConta { get; set; }

        [DataMember]
        public string BeneficiarioIban { get; set; }

        [DataMember]
        public string BeneficiarioSwift { get; set; }

        [DataMember]
        public string BeneficiarioBanco { get; set; }

        [DataMember]
        public decimal? BeneficiarioMontanteAPagar { get; set; }

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

        [DataMember]
        public List<ObligationItemDataContract> Items { get; set; } = new List<ObligationItemDataContract>();

        [DataMember]
        public List<ObligationBeneficiaryDataContract> Beneficiaries { get; set; } = new List<ObligationBeneficiaryDataContract>();
    }

    [DataContract]
    public class CompromissoComSaldoDataContract
    {
        [DataMember]
        public int CompromissoDespesaId { get; set; }

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

        [DataMember]
        public decimal SaldoDisponivel { get; set; }
    }
}

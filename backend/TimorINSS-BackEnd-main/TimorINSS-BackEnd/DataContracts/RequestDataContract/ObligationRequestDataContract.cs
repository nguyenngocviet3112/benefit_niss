using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetObligationListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class GetCompromissosComSaldoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class CreateObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public string DescritivoObrigacao { get; set; }

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
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class AddObligationBeneficiaryRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ObligationFk { get; set; }

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
    public class RemoveObligationBeneficiaryRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class AddObligationItemRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ObligationFk { get; set; }

        [DataMember]
        public int CompromissoDespesaFk { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class RemoveObligationItemRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class SubmitObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class ApproveObligationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}

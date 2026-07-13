using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetPaymentListRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class GetObligacoesDisponiveisParaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class CreatePaymentAuthorizationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ObligationFk { get; set; }

        [DataMember]
        public string Descritivo { get; set; }

        [DataMember]
        public decimal ValorAutorizado { get; set; }

        [DataMember]
        public int? CodigoContaDebitoFk { get; set; }

        [DataMember]
        public int? CodigoContaCreditoFk { get; set; }

        [DataMember]
        public int Mes { get; set; }

        [DataMember]
        public int Ano { get; set; }
    }

    [DataContract]
    public class SubmitPaymentAuthorizationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class ApprovePaymentAuthorizationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ExecutePaymentRequest : RequestBaseDataContract
    {
        [DataMember]
        public int PaymentAuthorizationFk { get; set; }

        [DataMember]
        public System.DateTime DataPagamento { get; set; }

        [DataMember]
        public int? ContaBancariaFk { get; set; }

        [DataMember]
        public string NumeroDocumento { get; set; }

        [DataMember]
        public string Observacao { get; set; }
    }

    // Bổ sung tài khoản Nợ/Có còn thiếu để ghi bù 1 bút toán đã bị bỏ qua lúc
    // Approve ("PaymentAuthorizationLiquidacao") hoặc Execute ("PaymentExecution")
    // — cấu hình tài khoản không bao giờ chặn nghiệp vụ chính, đây là đường
    // hoàn thiện sổ sách sau đó, ngay tại màn Pagamento (2026-07-13).
    [DataContract]
    public class CompletarLancamentoPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int PaymentAuthorizationFk { get; set; }

        // "PaymentAuthorizationLiquidacao" | "PaymentExecution"
        [DataMember(IsRequired = true)]
        public string OrigemTipo { get; set; }

        [DataMember(IsRequired = true)]
        public int CodigoContaDebitoFk { get; set; }

        [DataMember(IsRequired = true)]
        public int CodigoContaCreditoFk { get; set; }
    }
}

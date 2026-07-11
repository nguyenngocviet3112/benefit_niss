using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class PaymentAuthorizationListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<PaymentAuthorizationDataContract> Items { get; set; } = new List<PaymentAuthorizationDataContract>();
    }

    [DataContract]
    public class PaymentAuthorizationResponse : ResponseBaseDataContract
    {
        [DataMember]
        public PaymentAuthorizationDataContract Item { get; set; }
    }

    [DataContract]
    public class ObligacoesDisponiveisParaPagamentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ObligacaoDisponivelParaPagamentoDataContract> Items { get; set; } = new List<ObligacaoDisponivelParaPagamentoDataContract>();
    }

    [DataContract]
    public class CodigoContaOptionsResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CodigoContaOptionDataContract> Items { get; set; } = new List<CodigoContaOptionDataContract>();
    }

    [DataContract]
    public class ContaBancariaOptionsResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ContaBancariaOptionDataContract> Items { get; set; } = new List<ContaBancariaOptionDataContract>();
    }
}

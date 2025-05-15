using System;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class GuiaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember]
        public GuiaPagamentoDataContract GuiaPagamento { get; set; }

        [DataMember]
        public int IdContaCorrente { get; set; }
    }

    public class GetAllGuiasStatesFromYearByFilterRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int idEntidade { get; set; }
    }

    public class GetGuiaPagamentoRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int idGuiaPagamento { get; set; }
    }

    public class GetAllGuiasStatesFromDateByFilterRequest : SearchFilterRequest
    {
        [DataMember]
        public string niss { get; set; }
        [DataMember]
        public string paymentRef { get; set; }
        [DataMember]
        public string bankCode { get; set; }
    }

    public class UseCreditInGuiaPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int idEntidade { get; set; }

        [DataMember(IsRequired = true)]
        public int idGuia { get; set; }
    }

    public class insertComprovativoPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int idEntidade { get; set; }

        [DataMember(IsRequired = true)]
        public int idGuia { get; set; }

        [DataMember(IsRequired = true)]
        public decimal valorComprovativoPag { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime dataComprovativoPag { get; set; }

        [DataMember(IsRequired = true)]
        [Document]
        public string comprovativoPag { get; set; }
        [DataMember(IsRequired = true)]
        public string bankCode { get; set; }


    }

    public class approveComprovativoPagamentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int idEntidade { get; set; }

        [DataMember(IsRequired = true)]
        public int idGuia { get; set; }

        [DataMember(IsRequired = true)]
        public decimal valorComprovativoPag { get; set; }

        [DataMember(IsRequired = true)]
        public DateTime dataComprovativoPag { get; set; }

        [DataMember(IsRequired = true)]
        [Document]
        public string comprovativoPag { get; set; }
        [DataMember]
        public string rejectReason { get; set; }

        [DataMember]
        public int rejectStatus { get; set; }

    }
}
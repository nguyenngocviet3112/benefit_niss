using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelatorioPagamentosListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = false)]
        public string search { get; set; }

        [DataMember(IsRequired = false)]
        public int? estado { get; set; }

        [DataMember(IsRequired = false)]
        public string numeroPagamento { get; set; }

        [DataMember(IsRequired = false)]
        public int? contaOGE { get; set; }

        [DataMember(IsRequired = false)]
        public int? centroCusto { get; set; }
    }

    [DataContract]
    public class RelatorioContaOGEDropdownListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = false)]
        public int? centroCusto { get; set; }
    }
}
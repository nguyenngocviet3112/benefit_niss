using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class RelatorioGuiaPagamentoListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = false)]
        public string niss { get; set; }

        [DataMember(IsRequired = false)]
        public int? estado { get; set; }

        [DataMember(IsRequired = false)]
        public string numeroGuia { get; set; }
    }
}
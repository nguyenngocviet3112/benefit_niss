using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GuiaPagamentoContaConfigResponse : ResponseBaseDataContract
    {
        [DataMember]
        public GuiaPagamentoContaConfigDataContract Item { get; set; }
    }
}

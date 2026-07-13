using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveGuiaPagamentoContaConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int? CodigoContaCreditoPrivadoFk { get; set; }

        [DataMember]
        public int? CodigoContaCreditoPublicoFk { get; set; }
    }
}

using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class GuiaPagamentoContaConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? CodigoContaCreditoPrivadoFk { get; set; }

        [DataMember]
        public string CodigoContaCreditoPrivadoDesignacao { get; set; }

        [DataMember]
        public int? CodigoContaCreditoPublicoFk { get; set; }

        [DataMember]
        public string CodigoContaCreditoPublicoDesignacao { get; set; }
    }
}

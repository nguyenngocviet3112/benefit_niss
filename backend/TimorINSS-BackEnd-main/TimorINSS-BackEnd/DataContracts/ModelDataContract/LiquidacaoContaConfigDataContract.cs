using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class LiquidacaoContaConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Categoria { get; set; }

        [DataMember]
        public int? CodigoContaFk { get; set; }

        [DataMember]
        public string CodigoContaDesignacao { get; set; }
    }
}

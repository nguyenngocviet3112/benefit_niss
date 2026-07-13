using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveLiquidacaoContaConfigRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string Categoria { get; set; }

        [DataMember(IsRequired = true)]
        public int CodigoContaFk { get; set; }
    }
}

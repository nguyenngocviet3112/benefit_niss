using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetAgrupamentoConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int ContaCodigoFk { get; set; }

        [DataMember]
        public int TipoContaFK { get; set; }

        [DataMember]
        public int IdOrcamento { get; set; }
    }
}
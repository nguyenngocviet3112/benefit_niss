using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetDestinatarioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DestinatarioDataContract Destinatario;
    }
}
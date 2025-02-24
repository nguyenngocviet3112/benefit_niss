using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetDestinatarioRequest : RequestBaseDataContract
    {
        [DataMember]
        public string Niss { get; set; }

        [DataMember]
        public string Tin { get; set; }

        [DataMember]
        public string Nome { get; set; }
    }

    [DataContract]
    public class SaveDestinatarioRequest : RequestBaseDataContract
    {
        [DataMember]
        public DestinatarioDataContract destinatario;
    }
}
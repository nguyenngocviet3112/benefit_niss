using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class UserModeAccessResponse : ResponseBaseDataContract
    {
        [DataMember]
        public bool HasAccess { get; set; }
    }
}

using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class AttachmentConfigResponse : ResponseBaseDataContract
    {
        [DataMember]
        public AttachmentConfigDataContract item { get; set; }
    }
}

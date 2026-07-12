using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class AttachmentListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<AttachmentDataContract> items { get; set; } = new List<AttachmentDataContract>();
    }

    [DataContract]
    public class UploadAttachmentResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int id { get; set; }
    }
}

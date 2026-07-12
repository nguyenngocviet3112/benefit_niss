using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetAttachmentsByEntityRequest : RequestBaseDataContract
    {
        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public int EntityId { get; set; }
    }

    [DataContract]
    public class UploadAttachmentRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public string EntityType { get; set; }

        [DataMember(IsRequired = true)]
        public int EntityId { get; set; }

        [DataMember(IsRequired = true)]
        public string FileName { get; set; }

        [DataMember(IsRequired = true)]
        public string ContentType { get; set; }

        [DataMember(IsRequired = true)]
        public string FileContentBase64 { get; set; }
    }
}

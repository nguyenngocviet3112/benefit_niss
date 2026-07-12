using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    // Metadata only — FileContent (varbinary(max)) never travels through this
    // contract, only through the dedicated Download action (see AttachmentController).
    [DataContract]
    public class AttachmentDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string fileName { get; set; }

        [DataMember]
        public string contentType { get; set; }

        [DataMember]
        public int fileSize { get; set; }

        [DataMember]
        public DateTime dataCriacao { get; set; }
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ImportRowError
    {
        [DataMember]
        public int Row { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class ImportMasterDataTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int Total { get; set; }

        [DataMember]
        public int Success { get; set; }

        [DataMember]
        public int Failed { get; set; }

        [DataMember]
        public List<ImportRowError> RowErrors { get; set; } = new List<ImportRowError>();
    }
}

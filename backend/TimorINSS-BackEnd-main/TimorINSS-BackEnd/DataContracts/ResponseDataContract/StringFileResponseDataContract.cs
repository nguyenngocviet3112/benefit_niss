using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class StringFileReponse : ResponseBaseDataContract
    {
        [DataMember]
        public string File { get; set; }
    }
}
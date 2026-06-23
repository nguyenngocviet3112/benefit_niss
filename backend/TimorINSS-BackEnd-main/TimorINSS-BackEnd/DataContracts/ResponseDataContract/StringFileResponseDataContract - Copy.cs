using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ExcelImportReponse : ResponseBaseDataContract
    {
        [DataMember]
        public string Id { get; set; }
    }
}
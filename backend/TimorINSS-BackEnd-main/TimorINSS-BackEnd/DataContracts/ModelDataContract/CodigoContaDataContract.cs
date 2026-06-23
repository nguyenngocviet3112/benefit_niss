using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CodigoContaDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }
    }
}
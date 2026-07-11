using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveFunctionalClassificationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }
    }

    [DataContract]
    public class DeactivateFunctionalClassificationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}

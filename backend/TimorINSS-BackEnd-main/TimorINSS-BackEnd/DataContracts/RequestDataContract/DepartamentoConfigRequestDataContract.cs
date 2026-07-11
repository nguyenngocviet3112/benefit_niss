using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveDepartamentoConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }
    }

    [DataContract]
    public class DeactivateDepartamentoConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}

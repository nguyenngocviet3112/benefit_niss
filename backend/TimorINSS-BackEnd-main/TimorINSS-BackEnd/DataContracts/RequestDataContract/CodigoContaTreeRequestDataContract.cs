using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetCodigoContaTreeRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveCodigoContaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class DeactivateCodigoContaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}

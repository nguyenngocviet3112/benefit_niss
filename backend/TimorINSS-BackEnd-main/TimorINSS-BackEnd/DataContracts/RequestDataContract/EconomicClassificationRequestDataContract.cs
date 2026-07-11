using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetEconomicClassificationTreeRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveEconomicClassificationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int Nivel { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public string Tipo { get; set; }
    }

    [DataContract]
    public class DeactivateEconomicClassificationRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}

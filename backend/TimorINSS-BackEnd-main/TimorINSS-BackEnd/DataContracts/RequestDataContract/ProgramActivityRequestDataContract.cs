using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetProgramActivityTreeRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveProgramActivityRequest : RequestBaseDataContract
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
    }

    [DataContract]
    public class DeactivateProgramActivityRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class CopyProgramActivityYearRequest : RequestBaseDataContract
    {
        [DataMember]
        public int SourceOrcamentoConfigFk { get; set; }

        [DataMember]
        public int TargetOrcamentoConfigFk { get; set; }
    }
}

using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetAgrupamentoRubricaTreeRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        // "Receita" | "Despesa" | "Neutro Receita" | "Neutro Despesa"
        [DataMember]
        public string TipoConta { get; set; }
    }

    [DataContract]
    public class SaveAgrupamentoRubricaRequest : RequestBaseDataContract
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

        [DataMember]
        public string TipoConta { get; set; }
    }

    [DataContract]
    public class DeactivateAgrupamentoRubricaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }
}

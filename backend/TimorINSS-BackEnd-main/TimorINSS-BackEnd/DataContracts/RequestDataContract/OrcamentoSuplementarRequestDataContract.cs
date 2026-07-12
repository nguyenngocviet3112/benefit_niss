using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetActiveOrcamentoSuplementarRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class GetRubricasAprovadasParaSuplementarRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveOrcamentoSuplementarLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public int OrcamentoLinhaFk { get; set; }

        [DataMember]
        public decimal AdjustmentValue { get; set; }
    }

    [DataContract]
    public class DeleteOrcamentoSuplementarLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class SubmitOrcamentoSuplementarRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class ReviewOrcamentoSuplementarRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ApproveOrcamentoSuplementarRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}

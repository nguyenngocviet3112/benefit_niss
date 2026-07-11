using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetActiveOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class SaveOrcamentoLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public int AtividadeFk { get; set; }

        [DataMember]
        public int EconomicClassificationFk { get; set; }

        [DataMember]
        public int? FunctionalClassificationFk { get; set; }

        [DataMember]
        public int OrganizationFk { get; set; }

        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class DeleteOrcamentoLinhaRequest : RequestBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class SubmitOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int OrcamentoConfigFk { get; set; }
    }

    [DataContract]
    public class ReviewOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ApproveOrcamentoBatchRequest : RequestBaseDataContract
    {
        [DataMember]
        public int BatchId { get; set; }

        [DataMember]
        public bool Approve { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class ImportOrcamentoRequest : RequestBaseDataContract
    {
        public IFormFile File { get; set; }
        public int OrcamentoConfigFk { get; set; }
    }
}

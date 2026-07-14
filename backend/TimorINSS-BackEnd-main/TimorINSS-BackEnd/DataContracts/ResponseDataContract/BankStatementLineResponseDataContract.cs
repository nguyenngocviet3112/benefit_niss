using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class BankStatementLineListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<BankStatementLineDataContract> Items { get; set; } = new List<BankStatementLineDataContract>();
    }

    [DataContract]
    public class ReceitasDisponiveisParaConciliacaoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ReceitaDisponivelParaConciliacaoDataContract> Items { get; set; } = new List<ReceitaDisponivelParaConciliacaoDataContract>();
    }

    [DataContract]
    public class PagamentosDisponiveisParaConciliacaoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<PagamentoDisponivelParaConciliacaoDataContract> Items { get; set; } = new List<PagamentoDisponivelParaConciliacaoDataContract>();
    }

    [DataContract]
    public class ImportBankStatementLinePreviewResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<BankStatementLineImportRowDataContract> Rows { get; set; } = new List<BankStatementLineImportRowDataContract>();
    }
}

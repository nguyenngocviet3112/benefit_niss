using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class EntidadesRelatorioRequest : SearchFilterRequest
    {
        [DataMember]
        public bool? Ativo { get; set; }
    }

    [DataContract]
    public class SituacaoContributivaEmpresasRelatorioRequest : SearchFilterRequest
    {
        [DataMember]
        public string search { get; set; }

        [DataMember]
        public bool? apenasComDivida { get; set; }
    }

    [DataContract]
    public class ContribuicoesTrendsRelatorioRequest : SearchFilterRequest
    {
    }

    [DataContract]
    public class BudgetExecutionRelatorioRequest : SearchFilterRequest
    {
    }

    [DataContract]
    public class DespesaPipelineRelatorioRequest : SearchFilterRequest
    {
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class EntidadesRelatorioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<EntidadeRelatorioDataContract> entidades { get; set; }
    }

    [DataContract]
    public class SituacaoContributivaEmpresasRelatorioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<SituacaoContributivaEmpresaDataContract> empresas { get; set; }
    }

    [DataContract]
    public class ContribuicoesTrendsRelatorioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ContribuicoesTrendMesDataContract> meses { get; set; }
    }

    [DataContract]
    public class BudgetExecutionRelatorioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public decimal totalOrcado { get; set; }

        [DataMember]
        public decimal totalCabimentado { get; set; }

        [DataMember]
        public decimal totalCompromissado { get; set; }

        [DataMember]
        public decimal totalPago { get; set; }

        [DataMember]
        public List<RubricaOrcamentoDataContract> rubricasEmRisco { get; set; }
    }

    [DataContract]
    public class DespesaPipelineRelatorioResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaPipelineEstagioDataContract> estagios { get; set; }
    }
}

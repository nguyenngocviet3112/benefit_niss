using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteDespesaRegistoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaRegistadaDataContract> ComponenteDespesaRegisto;
    }

    [DataContract]
    public class GetValoresDespesaByIdCodigoOrcamentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ValoresDespesaRegistadaDataContract ValoresDespesa;
    }

    [DataContract]
    public class GetComponenteDespesaCabimentadaParaExecucaoReponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaCabimentadasParaExecucaoDataContract> DespesasParaExecucao;
    }

    [DataContract]
    public class GetDespesasRelatoriosReponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows;

        [DataMember]
        public List<DespesasRelatoriosDataContract> Despesas;
    }

    [DataContract]
    public class GetDespesasCompromissoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DespesaCompromissoDataContract> ComponenteDespesaObrigacao;
    }
}
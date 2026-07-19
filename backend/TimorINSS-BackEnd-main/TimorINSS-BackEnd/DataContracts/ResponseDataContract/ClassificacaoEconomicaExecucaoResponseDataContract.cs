using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ClassificacaoEconomicaExecucaoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ClassificacaoEconomicaExecucaoDataContract> lista;
    }
}

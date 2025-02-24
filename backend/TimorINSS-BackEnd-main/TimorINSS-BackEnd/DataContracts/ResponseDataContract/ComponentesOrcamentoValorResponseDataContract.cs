using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class SearchComponentesOrcamentoValorResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ComponenteOrcamentoValorFullDataContract> ValoresCorrentes { get; set; }
    }
}
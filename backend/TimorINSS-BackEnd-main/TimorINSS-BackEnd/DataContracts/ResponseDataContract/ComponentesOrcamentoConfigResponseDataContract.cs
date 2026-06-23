using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteOrcamentoConfigReponse : ResponseBaseDataContract
    {
        [DataMember]
        public ComponenteOrcamentoDataContract componenteOrcamentoConfig;
    }
}
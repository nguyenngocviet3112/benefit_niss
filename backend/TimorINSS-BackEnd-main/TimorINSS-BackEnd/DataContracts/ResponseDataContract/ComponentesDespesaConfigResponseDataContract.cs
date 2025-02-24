using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteDespesaConfigReponse : ResponseBaseDataContract
    {
        [DataMember]
        public ComponenteDespesaDataContract componenteDespesaConfig;
    }
}
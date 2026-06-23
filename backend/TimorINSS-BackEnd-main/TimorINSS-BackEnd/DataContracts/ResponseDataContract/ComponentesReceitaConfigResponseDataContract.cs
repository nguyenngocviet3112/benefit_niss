using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class GetComponenteReceitaConfigReponse : ResponseBaseDataContract
    {
        [DataMember]
        public ComponenteReceitaDataContract componenteReceitaConfig;
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class AgrupamentoConfigReponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<AgrupamentoConfigDataContract> Agrupamentos { get; set; }
    }
}
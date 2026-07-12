using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class AgrupamentoRubricaTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<AgrupamentoRubricaDataContract> Items { get; set; } = new List<AgrupamentoRubricaDataContract>();
    }
}

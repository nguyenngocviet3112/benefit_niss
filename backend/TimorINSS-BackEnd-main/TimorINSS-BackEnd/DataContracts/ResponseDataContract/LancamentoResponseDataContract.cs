using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class LancamentoListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<LancamentoDataContract> Items { get; set; } = new List<LancamentoDataContract>();
    }
}

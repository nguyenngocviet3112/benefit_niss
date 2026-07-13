using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class LiquidacaoContaConfigListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<LiquidacaoContaConfigDataContract> Items { get; set; } = new List<LiquidacaoContaConfigDataContract>();
    }
}

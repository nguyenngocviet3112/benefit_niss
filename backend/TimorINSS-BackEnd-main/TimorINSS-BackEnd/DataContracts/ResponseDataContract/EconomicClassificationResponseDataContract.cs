using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class EconomicClassificationTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<EconomicClassificationDataContract> Items { get; set; } = new List<EconomicClassificationDataContract>();
    }
}

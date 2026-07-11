using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class FunctionalClassificationTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<FunctionalClassificationDataContract> Items { get; set; } = new List<FunctionalClassificationDataContract>();
    }
}

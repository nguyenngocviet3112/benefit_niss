using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ProgramActivityTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ProgramActivityDataContract> Items { get; set; } = new List<ProgramActivityDataContract>();
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ReceitaPacListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ReceitaPacDataContract> Items { get; set; } = new List<ReceitaPacDataContract>();
    }

    [DataContract]
    public class ReceitaPacResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ReceitaPacDataContract Item { get; set; }
    }
}

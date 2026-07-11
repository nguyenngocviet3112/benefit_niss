using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CabimentoListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CabimentoDataContract> Items { get; set; } = new List<CabimentoDataContract>();
    }

    [DataContract]
    public class CabimentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public CabimentoDataContract Item { get; set; }
    }

    [DataContract]
    public class AdsDisponiveisParaCabimentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<AdDisponivelParaCabimentoDataContract> Items { get; set; } = new List<AdDisponivelParaCabimentoDataContract>();
    }
}

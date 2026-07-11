using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ObligationListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ObligationDataContract> Items { get; set; } = new List<ObligationDataContract>();
    }

    [DataContract]
    public class ObligationResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ObligationDataContract Item { get; set; }
    }

    [DataContract]
    public class CompromissosComSaldoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CompromissoComSaldoDataContract> Items { get; set; } = new List<CompromissoComSaldoDataContract>();
    }
}

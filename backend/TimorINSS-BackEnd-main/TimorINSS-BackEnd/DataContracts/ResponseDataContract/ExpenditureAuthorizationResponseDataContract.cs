using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ExpenditureAuthorizationListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ExpenditureAuthorizationDataContract> Items { get; set; } = new List<ExpenditureAuthorizationDataContract>();
    }

    [DataContract]
    public class ExpenditureAuthorizationResponse : ResponseBaseDataContract
    {
        [DataMember]
        public ExpenditureAuthorizationDataContract Item { get; set; }
    }

    [DataContract]
    public class AvailableRubricasResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<RubricaDisponivelDataContract> Items { get; set; } = new List<RubricaDisponivelDataContract>();
    }
}

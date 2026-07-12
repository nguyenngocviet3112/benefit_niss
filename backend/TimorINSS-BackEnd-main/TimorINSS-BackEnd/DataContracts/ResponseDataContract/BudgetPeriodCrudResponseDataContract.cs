using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class BudgetPeriodListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<BudgetPeriodDataContract> Items { get; set; } = new List<BudgetPeriodDataContract>();
    }
}

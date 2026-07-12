using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DashboardSummaryResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DashboardSummaryDataContract Summary { get; set; }
    }
}

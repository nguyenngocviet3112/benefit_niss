using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class OrcamentoBatchResponse : ResponseBaseDataContract
    {
        [DataMember]
        public OrcamentoBatchDataContract Batch { get; set; }
    }
}

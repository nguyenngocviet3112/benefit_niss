using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class IntegrationConfigResponse : ResponseBaseDataContract
    {
        [DataMember]
        public IntegrationConfigDataContract item { get; set; }
    }
}

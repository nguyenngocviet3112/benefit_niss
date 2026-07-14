using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveIntegrationConfigRequest : RequestBaseDataContract
    {
        [DataMember]
        public bool BenefitApiEnabled { get; set; }
    }
}

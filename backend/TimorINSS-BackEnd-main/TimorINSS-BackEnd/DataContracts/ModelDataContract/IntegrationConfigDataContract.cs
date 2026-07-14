using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class IntegrationConfigDataContract
    {
        [DataMember]
        public bool benefitApiEnabled { get; set; }
    }
}

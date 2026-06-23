using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    public class INSSEstrangeiroRequest : RequestBaseDataContract
    {
        [DataMember]
        [ContainsDocument]
        public INSSEstrangeiroDataContract INSSEstrangeiro { get; set; }
    }
}
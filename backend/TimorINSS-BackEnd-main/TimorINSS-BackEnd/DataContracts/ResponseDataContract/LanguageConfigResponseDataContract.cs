using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class LanguageConfigListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<LanguageConfigDataContract> Items { get; set; } = new List<LanguageConfigDataContract>();
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CicloDespesaListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CicloDespesaDataContract> items { get; set; } = new List<CicloDespesaDataContract>();
    }
}

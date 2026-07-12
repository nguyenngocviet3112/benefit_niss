using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CodigoContaOpeningBalanceListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CodigoContaOpeningBalanceDataContract> Items { get; set; } = new List<CodigoContaOpeningBalanceDataContract>();
    }
}

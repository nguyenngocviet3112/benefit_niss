using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CodigoContaTreeResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CodigoContaTreeItemDataContract> Items { get; set; } = new List<CodigoContaTreeItemDataContract>();
    }
}

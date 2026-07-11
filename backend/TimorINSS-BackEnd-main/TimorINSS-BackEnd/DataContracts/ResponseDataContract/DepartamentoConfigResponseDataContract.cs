using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DepartamentoConfigListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<DepartamentoConfigDataContract> Items { get; set; } = new List<DepartamentoConfigDataContract>();
    }

    [DataContract]
    public class DepartamentoConfigResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DepartamentoConfigDataContract Item { get; set; }
    }
}

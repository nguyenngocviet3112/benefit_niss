using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class OrcamentoSuplementarBatchResponse : ResponseBaseDataContract
    {
        [DataMember]
        public OrcamentoSuplementarBatchDataContract Batch { get; set; }
    }

    [DataContract]
    public class RubricasAprovadasParaSuplementarResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<RubricaAprovadaParaSuplementarDataContract> Items { get; set; } = new List<RubricaAprovadaParaSuplementarDataContract>();
    }
}

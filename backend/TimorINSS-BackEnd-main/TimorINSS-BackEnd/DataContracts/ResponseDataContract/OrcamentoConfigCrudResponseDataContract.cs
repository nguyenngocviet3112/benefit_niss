using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    // Named distinctly from the pre-existing OrcamentoResponseDataContract.cs
    // (OrcamentoBatchResponse, used by the M2 OrcamentoController) to avoid
    // any confusion between the two unrelated controllers.
    [DataContract]
    public class OrcamentoConfigListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<OrcamentoConfigDataContract> Items { get; set; } = new List<OrcamentoConfigDataContract>();
    }
}

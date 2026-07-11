using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class CompromissoDespesaListResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CompromissoDespesaDataContract> Items { get; set; } = new List<CompromissoDespesaDataContract>();
    }

    [DataContract]
    public class CompromissoDespesaResponse : ResponseBaseDataContract
    {
        [DataMember]
        public CompromissoDespesaDataContract Item { get; set; }
    }

    [DataContract]
    public class CabimentosDisponiveisResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<CabimentoDisponivelParaCompromissoDataContract> Items { get; set; } = new List<CabimentoDisponivelParaCompromissoDataContract>();
    }
}

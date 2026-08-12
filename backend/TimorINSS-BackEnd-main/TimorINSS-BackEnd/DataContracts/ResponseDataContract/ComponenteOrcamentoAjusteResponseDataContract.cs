using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class SolicitarAjusteOrcamentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class GetAjustesOrcamentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<ComponenteOrcamentoAjusteDataContract> Ajustes { get; set; } = new List<ComponenteOrcamentoAjusteDataContract>();
    }

    [DataContract]
    public class GetRubricasDisponiveisResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int? ComponenteOrcamentoRegistoFk { get; set; }

        [DataMember]
        public List<RubricaDisponivelDataContract> Rubricas { get; set; } = new List<RubricaDisponivelDataContract>();
    }
}

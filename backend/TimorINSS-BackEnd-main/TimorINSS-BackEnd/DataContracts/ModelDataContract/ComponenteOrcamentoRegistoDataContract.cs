using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteOrcamentoRegistoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int TarefaActivoFk { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime DataFim { get; set; }

        [DataMember]
        public bool Aprovado { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public int? OrcamentoRetificadoFk { get; set; }
    }
}
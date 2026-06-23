using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class GetComponenteOrcamentoRegistoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdTarefaActivo { get; set; }
    }

    [DataContract]
    public class UpdateComponenteOrcamentoRegistoDatesRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdTarefaActivo { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime DataFim { get; set; }
    }

    [DataContract]
    public class GetComponenteOrcamentoRegistoAprovadoRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdTarefaActivo { get; set; }
    }

    [DataContract]
    public class OrcamentoExtractRequest : RequestBaseDataContract
    {
        [DataMember]
        public int IdComponenteOrcamentoRegisto { get; set; }

        [DataMember]
        public List<int> FiltrosDepartamento { get; set; }

        [DataMember]
        public List<int> FiltrosCentroDeCusto { get; set; }

        [DataMember]
        public List<int> FiltrosTipoDeConta { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ComponenteHistoricoTextoListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public List<HistoryText> historicoTextos;
    }

    [DataContract]
    public class HistoryText
    {
        [DataMember]
        public string user { get; set; }

        [DataMember]
        public string tarefa { get; set; }

        [DataMember]
        public string titulo { get; set; }

        [DataMember]
        public string texto { get; set; }

        [DataMember]
        public DateTime data { get; set; }
    }
}
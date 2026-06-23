using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ProcessoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime data { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public bool indAtivo { get; set; }
    }

    [DataContract]
    public class ProcessoDetalhe
    {
        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public int quantidadeTarefas { get; set; }

        [DataMember]
        public DateTime data { get; set; }

        [DataMember]
        public bool arquivado { get; set; }

        [DataMember]
        public string nomeUtilizadorUltimaEdicao { get; set; }
    }
}
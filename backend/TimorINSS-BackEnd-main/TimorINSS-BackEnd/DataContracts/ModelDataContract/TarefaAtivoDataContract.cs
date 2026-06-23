using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class TarefaAtivoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string dataInicioProcesso { get; set; }

        [DataMember]
        public string nomeProcesso { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public int estado { get; set; }

        [DataMember]
        public int estadoDays { get; set; }

        [DataMember]
        public DateTime ultimaAtualizacao { get; set; }

        [DataMember]
        public string numeroProcesso { get; set; }
    }
}
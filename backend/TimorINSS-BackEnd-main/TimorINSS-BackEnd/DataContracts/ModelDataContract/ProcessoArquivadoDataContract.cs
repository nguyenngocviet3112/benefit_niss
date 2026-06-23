using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ProcessoArquivadoDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime data { get; set; }

        [DataMember]
        public string nome { get; set; }

        [DataMember]
        public string numero { get; set; }
    }
}
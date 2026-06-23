using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CompromissoDataContract
    {
        [DataMember]
        public int? id { get; set; }

        [DataMember]
        public int despesaRegistadaFk { get; set; }

        [DataMember]
        public string nomeCompromisso { get; set; }

        [DataMember]
        public decimal valorCompromisso { get; set; }

        [DataMember]
        public DateTime dataCompromisso { get; set; }
    }
}

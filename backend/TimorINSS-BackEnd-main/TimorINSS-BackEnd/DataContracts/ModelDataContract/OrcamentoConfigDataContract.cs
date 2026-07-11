using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class OrcamentoConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Ano { get; set; }

        [DataMember]
        public string Tipo { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime? DataFim { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }
    }
}

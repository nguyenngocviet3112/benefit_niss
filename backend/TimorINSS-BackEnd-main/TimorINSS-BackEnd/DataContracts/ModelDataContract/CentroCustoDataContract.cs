using System;
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class CentroCustoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public DateTime DataInicio { get; set; }

        [DataMember]
        public DateTime? DataFim { get; set; }

        [DataMember]
        public int OrcamentoconfigFk { get; set; }
    }
}
using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class AgrupamentoConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int TipoDeConta { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }

        [DataMember]
        public bool Final { get; set; }
    }
}
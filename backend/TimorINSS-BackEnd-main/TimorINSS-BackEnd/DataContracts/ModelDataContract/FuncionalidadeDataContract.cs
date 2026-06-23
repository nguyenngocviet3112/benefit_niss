using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class FuncionalidadeDataContract
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string descricao { get; set; }

        [DataMember]
        public bool create { get; set; }

        [DataMember]
        public bool read { get; set; }

        [DataMember]
        public bool update { get; set; }

        [DataMember]
        public bool delete { get; set; }
    }
}
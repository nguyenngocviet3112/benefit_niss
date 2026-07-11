using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class LanguageConfigDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public int Ordem { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }
    }
}

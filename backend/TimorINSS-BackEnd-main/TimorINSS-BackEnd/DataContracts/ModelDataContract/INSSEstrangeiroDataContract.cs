using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class INSSEstrangeiroDataContract
    {
        [DataMember]
        public int IdInssestrang { get; set; }

        [DataMember]
        public int? IdEntidade { get; set; }

        [DataMember]
        public int? IdTrabalhador { get; set; }

        [DataMember]
        public string NomeSSEstrangeiro { get; set; }

        [DataMember]
        public int EstrangeiroPaisFk { get; set; }

        [DataMember]
        public bool IndDecontAtualmente { get; set; }

        [DataMember]
        public bool IndBenfAtualmente { get; set; }

        [DataMember]
        [Document]
        public string Documento { get; set; }

        [DataMember]
        public string NomeDocumento { get; set; }

        [DataMember]
        public string Nissestrangeiro { get; set; }
    }
}
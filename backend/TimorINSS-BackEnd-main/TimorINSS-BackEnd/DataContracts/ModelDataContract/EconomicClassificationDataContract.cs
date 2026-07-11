using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class EconomicClassificationDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public string Designacao { get; set; }

        [DataMember]
        public int Nivel { get; set; }

        [DataMember]
        public int? ParentFk { get; set; }

        [DataMember]
        public int OrcamentoConfigFk { get; set; }

        [DataMember]
        public bool IndActivo { get; set; }

        [DataMember]
        public bool HasKids { get; set; }

        [DataMember]
        public string Tipo { get; set; }
    }
}

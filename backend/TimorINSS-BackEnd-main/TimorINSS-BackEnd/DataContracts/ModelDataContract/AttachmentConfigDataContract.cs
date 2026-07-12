using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class AttachmentConfigDataContract
    {
        [DataMember]
        public int maxFileSizeMb { get; set; }

        [DataMember]
        public bool adObrigatorio { get; set; }

        [DataMember]
        public bool cabimentoObrigatorio { get; set; }

        [DataMember]
        public bool compromissoObrigatorio { get; set; }

        [DataMember]
        public bool obrigacaoObrigatorio { get; set; }

        [DataMember]
        public bool pagamentoObrigatorio { get; set; }
    }
}

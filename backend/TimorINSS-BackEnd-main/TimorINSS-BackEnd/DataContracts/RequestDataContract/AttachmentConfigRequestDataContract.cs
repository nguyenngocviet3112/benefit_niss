using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class SaveAttachmentConfigRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int MaxFileSizeMb { get; set; }

        [DataMember]
        public bool AdObrigatorio { get; set; }

        [DataMember]
        public bool CabimentoObrigatorio { get; set; }

        [DataMember]
        public bool CompromissoObrigatorio { get; set; }

        [DataMember]
        public bool ObrigacaoObrigatorio { get; set; }

        [DataMember]
        public bool PagamentoObrigatorio { get; set; }
    }
}

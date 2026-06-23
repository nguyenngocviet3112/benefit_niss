using System.Runtime.Serialization;

namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    [DataContract]
    public class ComponenteOrcamentoDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int TarefaFk { get; set; }

        [DataMember]
        public int PermissaoDatas { get; set; }

        [DataMember]
        public int PermissaoInsercoes { get; set; }

        [DataMember]
        public int PermissaoDetalhes { get; set; }

        [DataMember]
        public int PermissaoAprovacao { get; set; }
    }
}
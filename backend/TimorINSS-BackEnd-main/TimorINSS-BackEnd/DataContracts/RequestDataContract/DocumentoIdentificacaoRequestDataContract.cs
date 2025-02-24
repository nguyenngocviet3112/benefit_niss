using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.RequestDataContract
{
    [DataContract]
    public class DocumentosListagemRequest : SearchFilterRequest
    {
        [DataMember(IsRequired = true)]
        public int Id { get; set; }
    }

    public class DocumentoIdRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int id { get; set; }
    }

    public class DocumentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        [ContainsDocument]
        public DocumentoIdentificacaoDataContract documento { get; set; }
    }

    public class TarefaDocumentoRequest : RequestBaseDataContract
    {
        [DataMember(IsRequired = true)]
        public int tarefaAtivoId { get; set; }

        [DataMember(IsRequired = true)]
        public int tipoDocumento { get; set; }

        [DataMember(IsRequired = true)]
        public string nomeDocumento { get; set; }

        [DataMember]
        [Document]
        public string documento { get; set; }
    }
}
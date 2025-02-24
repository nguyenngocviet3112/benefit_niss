using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class DocumentosListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<DocumentosListagem> documentos { get; set; }
    }

    [DataContract]
    public class DocumentosListagem
    {
        [DataMember]
        public int IdDocumento { get; set; }

        [DataMember]
        public string TpDocIdentificacao { get; set; }

        [DataMember]
        public string Numero { get; set; }

        [DataMember]
        public DateTime? DataValidade { get; set; }

        [DataMember]
        public byte[] Documento { get; set; }
    }

    [DataContract]
    public class DocumentoResponse : ResponseBaseDataContract
    {
        [DataMember]
        public DocumentoIdentificacaoDataContract documento { get; set; }
    }

    [DataContract]
    public class DocumentosTarefaListagemResponse : ResponseBaseDataContract
    {
        [DataMember]
        public int rows { get; set; }

        [DataMember]
        public List<DocumentosTarefaListagem> documentos { get; set; }
    }

    [DataContract]
    public class DocumentosTarefaListagem
    {
        [DataMember]
        public int IdDocumento { get; set; }

        [DataMember]
        public string TpDocIdentificacao { get; set; }

        [DataMember]
        public string Numero { get; set; }

        [DataMember]
        public DateTime? DataCriacao { get; set; }

        [DataMember]
        public string Tarefa { get; set; }

        [DataMember]
        public string Utilizador { get; set; }

        [DataMember]
        public byte[] Documento { get; set; }
    }
}
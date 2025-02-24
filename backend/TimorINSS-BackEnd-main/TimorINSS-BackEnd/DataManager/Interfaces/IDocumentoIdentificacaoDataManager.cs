using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDocumentoIdentificacaoDataManager
    {
        public DocumentosListagemResponse GetDocumentosIdentificacaoByIdTrabalhador(DocumentosListagemRequest request);

        public DocumentoResponse GetDocumentosIdentificacaoById(DocumentoIdRequest documento);

        public ResponseBaseDataContract SaveDocumento(DocumentoRequest documento);

        public ResponseBaseDataContract UpdateDocumento(DocumentoRequest documento);

        public ResponseBaseDataContract DeleteDocumento(DocumentoIdRequest documento);

        public DocumentosTarefaListagemResponse getDocumentosByIdTarefaAtivo(DocumentosListagemRequest request);

        public DocumentosTarefaListagemResponse getDocumentosByIdProcessoAtivo(DocumentosListagemRequest request);

        public ResponseBaseDataContract SaveTarefaDocumento(TarefaDocumentoRequest request);

        public ResponseBaseDataContract DeleteDocumentoComponente(DocumentoIdRequest request);
    }
}
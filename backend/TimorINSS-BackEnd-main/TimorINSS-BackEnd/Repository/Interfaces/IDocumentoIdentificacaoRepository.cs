using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDocumentoIdentificacaoRepository : IDataRepository<Documentoidentificacao, DocumentoidentificacaoDto>
    {
        public List<Documentoidentificacao> GetByTrabalhadorEntidade(long id);

        public List<Documentoidentificacao> GetByResponsavelLegalEntidade(long id);

        public DocumentosListagemResponse GetListagemByFilter(DocumentosListagemRequest request);

        public List<Documentoidentificacao> GetDocumentoidentificacaoByTipoENumero(int tipoDocumento, string numero);

        public List<Documentoidentificacao> GetDocumentoidentificacaoByTrabalhadorFkETipo(int trabalhadorFk, int tipo);
    }
}
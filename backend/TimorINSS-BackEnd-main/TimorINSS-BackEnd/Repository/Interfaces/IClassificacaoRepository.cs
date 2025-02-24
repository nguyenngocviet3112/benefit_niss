using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IClassificacaoRepository : IDataRepository<Classificacao, ClassificacaoDto>
    {
        public List<SelectDescription> GetAllClassificacao();

        public ValueCampoEditavelListagemResponse GetAllActiveClassificacao(SearchFilter filter);

        public List<SelectDescription> GetAllActiveClassificacao();
    }
}
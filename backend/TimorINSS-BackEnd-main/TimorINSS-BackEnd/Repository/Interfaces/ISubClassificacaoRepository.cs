using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ISubClassificacaoRepository : IDataRepository<Subclassificacao, SubClassificacaoDto>
    {
        public List<SelectDescription> GetAllSubClassificacao();

        public ValueCampoEditavelListagemResponse GetAllActiveSubClassificacao(SearchFilter filter);
    }
}
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelTipoDeContaOrcamentoConfigRepository : IDataRepository<Reltipodecontaorcamentoconfig, RelTipoDeContaOrcamentoConfigDto>
    {
        public ValueCampoEditavelListagemResponse GetAllActiveRelTipoDeContaOrcamentoConfig(SearchFilter filter);

        public List<SelectDescription> GetAllRelTipoDeContaOrcamentoConfigByParent(int parent);
    }
}
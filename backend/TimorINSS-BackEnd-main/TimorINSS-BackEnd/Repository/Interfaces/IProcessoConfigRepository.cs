using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IProcessoConfigRepository : IDataRepository<Processoconfig, ProcessoconfigDto>
    {
        public ProcessosListagemResponse GetAllProcessos(SearchFilterRequest request);

        public Processoconfig GetWithActiveRelations(long id);

        public SelectDescriptionResponse ListIniciarProcessos(List<int> allowedProcessConfigs, bool isAdmin = false);

        public SelectDescriptionResponse ListIniciarApprove(List<int> allowedProcessConfigs, bool isAdmin = false);
    }
}
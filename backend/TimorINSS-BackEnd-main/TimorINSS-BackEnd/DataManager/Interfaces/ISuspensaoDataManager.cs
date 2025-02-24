using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ISuspensaoDataManager
    {
        public IEnumerable<Suspensoes> GetAll();

        public Suspensoes Get(long id);

        public SuspensoesDto GetDto(long id);

        public SuspensaoListagemResponse GetSuspensaoByIdEntidadeEmpregadora(SuspensaoListagemRequest request);

        public SuspensaoListagemResponse GetSuspensaoByIdTrabalhador(SuspensaoListagemRequest request);

        public void Add(Suspensoes entity);

        public void Update(Suspensoes entity);

        public void Delete(Suspensoes entity);

        public SuspensaoResponse SaveSuspensao(SuspensaoRequest suspensao);

        public ResponseBaseDataContract DeleteSuspensao(SuspensaoDeleteRequest request);
    }
}
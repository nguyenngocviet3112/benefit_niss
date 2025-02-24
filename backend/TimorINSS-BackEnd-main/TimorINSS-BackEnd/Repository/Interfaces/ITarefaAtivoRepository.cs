using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ITarefaAtivoRepository : IDataRepository<Tarefaativo, TarefaativoDto>
    {
        public TarefasAtivasListagemResponse GetAllTarefasAtivas(SearchFilterRequest request, List<int> allowedTarefaIds);

        public List<int> GetAllTarefasAtivasSemControlo();

        public Tarefaativo GetTarefaAtivoById(long id);

        public List<int> GetAllTarefasIdsByProcessoAtivoIdExcludingCurrent(long processoAtivoId, long currentTarefaId);

        public List<int> GetAllTarefasIdsByProcessoAtivoId(long processoAtivoId);
    }
}
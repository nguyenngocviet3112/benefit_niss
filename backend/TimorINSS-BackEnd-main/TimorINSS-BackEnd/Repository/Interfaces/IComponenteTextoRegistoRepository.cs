using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteTextoRegistoRepository : IDataRepository<ComponentetextoRegisto, ComponentetextoRegistoDto>
    {
        public List<HistoryText> GetHistoryTextsByTarefaAtivoIds(List<int> tarefaIds);

        public IEnumerable<ComponentetextoRegisto> GetAllByTarefaAtivoId(int tarefaAtivoId);
    }
}
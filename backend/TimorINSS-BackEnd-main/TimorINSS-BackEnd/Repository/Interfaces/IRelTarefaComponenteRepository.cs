using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelTarefaComponenteRepository : IDataRepository<Reltarefacomponente, RelTarefaComponenteDto>
    {
        public List<Componentes> GetRelTarefaComponenteByIdTarefa(int idTarefa);

        public Reltarefacomponente GetRelTarefaComponenteByIdTarefaIdComponente(int idComponente, int idTarefa);
    }
}
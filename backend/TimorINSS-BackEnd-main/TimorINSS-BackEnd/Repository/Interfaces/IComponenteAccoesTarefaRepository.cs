using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteAccoesTarefaRepository : IDataRepository<Componenteaccoestarefa, ComponenteAccoesTarefaDto>
    {
        public List<ComponenteAccoesTarefa> GetByIdTarefa(int idTarefa);

        public List<Componenteaccoestarefa> GetComponenteByIdTarefa(int idTarefa);

        public List<SelectDescription> GetAllTarefasASeguir(int idTarefa);
    }
}
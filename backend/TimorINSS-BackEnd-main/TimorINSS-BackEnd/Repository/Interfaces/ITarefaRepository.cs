using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ITarefaRepository : IDataRepository<Tarefa, TarefaDto>
    {
        public TarefaListagemResponse GetAllTarefas(SearchFilterRequest request);

        public List<SelectDescription> GetAllTarefaAtivo();

        public string GetNextNumeroTarefa();
    }
}
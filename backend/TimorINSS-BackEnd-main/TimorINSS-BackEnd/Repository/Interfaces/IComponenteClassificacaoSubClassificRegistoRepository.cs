using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteClassificacaoSubClassificRegistoRepository : IDataRepository<ComponenteclassificacaosubRegisto, ComponenteclassificacaosubRegistoDto>
    {
        public ComponenteclassificacaosubRegisto GetByTarefaAtivoId(int tarefaAtivoId);
    }
}
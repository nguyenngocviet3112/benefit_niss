using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteOrcamentoRepository : IDataRepository<Componenteorcamento, ComponenteOrcamentoDto>
    {
        public Componenteorcamento? GetByIdTarefa(int id);
    }
}
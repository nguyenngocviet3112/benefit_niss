using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteDespesaRepository : IDataRepository<Componentedespesa, ComponenteDespesaDto>
    {
        public Componentedespesa? GetByIdTarefa(int id);
    }
}
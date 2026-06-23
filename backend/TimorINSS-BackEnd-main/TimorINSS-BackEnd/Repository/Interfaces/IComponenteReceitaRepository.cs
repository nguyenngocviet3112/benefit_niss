using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteReceitaRepository : IDataRepository<Componentereceita, ComponenteReceitaDto>
    {
        public Componentereceita? GetByIdTarefa(int id);
    }
}
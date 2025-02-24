using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteConciliacaoMovimentosRepository : IDataRepository<Componenteconciliacaomovimentos, ComponenteconciliacaomovimentosDto>
    {
        public Componenteconciliacaomovimentos? GetByIdTarefa(int id);
    }
}
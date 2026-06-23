using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteRepository : IDataRepository<Componente, ComponenteDto>
    {
        public ComponentesListagemResponse GetAllComponentes();
    }
}
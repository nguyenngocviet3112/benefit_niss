using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteTextoRepository : IDataRepository<Componentetexto, ComponenteTextoDto>
    {
        public ComponenteTexto GetByIdTarefa(int idTarefa);

        public Componentetexto GetComponenteByIdTarefa(int idTarefa);
    }
}
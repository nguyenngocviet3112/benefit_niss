using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteCarregarDocumentoRepository : IDataRepository<Componentecarregardocumento, ComponenteCarregarDocumentoDto>
    {
        public List<ComponenteDocumentoTarefa> GetByIdTarefa(int idTarefa);

        public List<Componentecarregardocumento> GetComponenteByIdTarefa(int idTarefa);
    }
}
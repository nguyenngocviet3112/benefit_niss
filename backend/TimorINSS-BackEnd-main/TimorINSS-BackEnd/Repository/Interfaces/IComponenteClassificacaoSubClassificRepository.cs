using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteClassificacaoSubClassificRepository : IDataRepository<Componenteclassificacaosubclassific, ComponenteClassificacaoSubClassificDto>
    {
        public List<ComponenteClassificacaoSubClassificTarefa> GetByIdTarefa(int idTarefa);

        public List<Componenteclassificacaosubclassific> GetComponenteByIdTarefa(int idTarefa);
    }
}
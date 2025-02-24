using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IAldeiaRepository : IDataRepository<Aldeia, AldeiaDto>
    {
        public List<SelectDescription> getAllAldeia();

        public List<SelectDescription> getAldeiaByIdSuco(int id);

        public ValueCampoEditavelListagemResponse getAllActiveAldeia(SearchFilter filter);
    }
}
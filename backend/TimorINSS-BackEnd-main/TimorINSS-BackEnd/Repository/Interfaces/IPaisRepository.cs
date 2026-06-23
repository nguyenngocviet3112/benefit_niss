using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPaisRepository : IDataRepository<Pais, PaisDto>
    {
        public List<SelectDescription> getAllPais();

        public ValueCampoEditavelListagemResponse getAllActivePais(SearchFilter filter);
    }
}
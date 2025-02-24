using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ISucoRepository : IDataRepository<Suco, SucoDto>
    {
        public List<SelectDescription> getAllSuco();

        public List<SelectDescription> getAllActiveSuco();

        public List<SelectDescription> getSucoByIdPosto(int id);

        public ValueCampoEditavelListagemResponse getAllActiveSuco(SearchFilter filter);

        public Suco GetWithActiveChilds(long id);
    }
}
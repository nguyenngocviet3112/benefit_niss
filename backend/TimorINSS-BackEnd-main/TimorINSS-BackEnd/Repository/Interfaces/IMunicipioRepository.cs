using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IMunicipioRepository : IDataRepository<Municipio, MunicipioDto>
    {
        public List<SelectDescription> getAllMunicipio();

        public List<SelectDescription> getAllActiveMunicipio();

        public ValueCampoEditavelListagemResponse getAllActiveMunicipio(SearchFilter filter);

        public Municipio GetWithActiveChilds(long id);
    }
}
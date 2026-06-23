using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ISectorActividadeRepository : IDataRepository<Sectoractividade, SectoractividadeDto>
    {
        public List<SelectDescription> GetAllSectorActividade();

        public ValueCampoEditavelListagemResponse GetAllActiveSectorActividade(SearchFilter filter);
    }
}
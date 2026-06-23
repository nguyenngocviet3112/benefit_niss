using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IActividadeEconomicaRepository : IDataRepository<Actividadeeconomica, ActividadeeconomicaDto>
    {
        public List<SelectDescription> GetAllActividadeEconomica();

        public ValueCampoEditavelListagemResponse GetAllActiveActividadeEconomica(SearchFilter filter);

        public bool DoesCodeExists(int id, string code);
    }
}
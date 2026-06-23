using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface INaturezaJuridicaRepository : IDataRepository<Naturezajuridica, NaturezajuridicaDto>
    {
        public List<SelectDescription> GetAllNaturezaJuridica();

        public ValueCampoEditavelListagemResponse GetAllActiveNaturezaJuridica(SearchFilter filter);

        public bool DoesCodeExists(int id, string code);
    }
}
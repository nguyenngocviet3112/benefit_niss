using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPerfilRepository : IDataRepository<Perfil, PerfilDto>
    {
        public PerfilListagemResponse GetAllPerfis(SearchFilterRequest request);

        public List<SelectDescription> GetAllPerfisAtivo();

        public List<Perfil> GetPerfisByIds(List<int> perfilIds);

        public List<int> GetActivePerfisByIds(List<int> perfilIds);
    }
}
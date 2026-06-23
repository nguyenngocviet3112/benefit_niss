using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelUtilizadorPerfilRepository : IDataRepository<Relutilizadorperfil, RelUtilizadorPerfilDto>
    {
        public List<int> GetPerfisIdsByUtilizador(int userId);

        public List<Relutilizadorperfil> GetRelUtilizadorPerfilByUserId(int userId);

        public PerfilListagemResponse GetPerfisByUserId(int id);

        public Relutilizadorperfil GetRelByPerfilId(int idPerfil);
    }
}
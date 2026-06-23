using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelProcessoConfigPerfilRepository : IDataRepository<Relprocessoconfigperfil, RelprocessoconfigperfilDto>
    {
        public List<Relprocessoconfigperfil> GetRelPerfilByProcesso(long processoId);

        public List<int> GetProcessoIdsByPerfis(List<int> perfilIds);
    }
}
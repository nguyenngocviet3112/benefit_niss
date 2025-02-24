using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelPerfilFuncionalidadeRepository : IDataRepository<Relperfilfuncionalidade, RelPerfilFuncionalidadeDto>
    {
        public List<Relperfilfuncionalidade> GetFuncionalidadeByPerfil(List<int> perfilIds);

        public List<FuncionalidadeDataContract> GetRelPerfilFuncionalidadeByIdPerfil(int idPerfil);
    }
}
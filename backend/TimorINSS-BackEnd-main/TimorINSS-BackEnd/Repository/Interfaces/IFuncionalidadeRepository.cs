using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IFuncionalidadeRepository : IDataRepository<Funcionalidade, FuncionalidadeDto>
    {
        public List<FuncionalidadeDataContract> GetAllFuncionalidades();
    }
}
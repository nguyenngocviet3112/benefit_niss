using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteReceitaRegistoMovimentosRepository : IDataRepository<ComponentereceitaRegistoMovimentos, ComponenteReceitaRegistoMovimentosDto>
    {
        public List<ComponentereceitaRegistoMovimentos> GetMovimentosByIdReceita(int idReceita);
    }
}
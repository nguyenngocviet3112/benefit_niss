using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelMovimentosporconciliarMovimentosRepository : IDataRepository<RelMovimentosporconciliarMovimentos, RelMovimentosporconciliarMovimentosDto>
    {
        public IEnumerable<RelMovimentosporconciliarMovimentos> GetAllFromMovimentoBancario(int id);

        public IEnumerable<RelMovimentosporconciliarMovimentos> GetAllFromMovimentosPorConciliar(int id, MovimentosPorConciliarListagemType type);
    }
}
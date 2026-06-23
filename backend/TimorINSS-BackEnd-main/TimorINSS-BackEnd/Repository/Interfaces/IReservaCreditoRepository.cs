using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IReservaCreditoRepository : IDataRepository<Reservacredito, ReservaCreditoDto>
    {
        public ReservaCreditoListagemResponse GetReservaCreditoByIdEntidade(ReservaCreditoListagemRequest request);

        public Reservacredito GetActiveByEntidadeId(long entidadeId);

        public List<Reservacredito> GetByIds(List<int> reservaIds);
    }
}
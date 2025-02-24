using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IReservaCreditoDataManager
    {
        public IEnumerable<Reservacredito> GetAll();

        public Reservacredito Get(long id);

        public ReservaCreditoDto GetDto(long id);

        public ReservaCreditoListagemResponse GetReservaCreditoByIdEntidade(ReservaCreditoListagemRequest request);
    }
}
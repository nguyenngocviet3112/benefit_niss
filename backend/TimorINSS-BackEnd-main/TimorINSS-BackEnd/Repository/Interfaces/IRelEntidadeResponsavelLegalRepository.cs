using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IRelEntidadeResponsavelLegalRepository : IDataRepository<Relentidaderesplegal, RelentidaderesplegalDto>
    {
        public RelentidaderesplegalDto GetDtoByResponsavelLegal(long id);

        public Relentidaderesplegal GetByResponsavelLegal(long id);
    }
}
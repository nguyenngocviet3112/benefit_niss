using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDispensaContributivaRepository : IDataRepository<Dispensacontributiva, DispensaContributivaDto>
    {
        public decimal getDispensaContributivaByYear(int year);
    }
}
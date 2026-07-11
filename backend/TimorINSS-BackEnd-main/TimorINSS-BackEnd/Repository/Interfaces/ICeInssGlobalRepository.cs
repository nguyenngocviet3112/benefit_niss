using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICeInssGlobalRepository
    {
        CeInssGlobalResponse GetReport(CeInssGlobalRequest request);
    }
}

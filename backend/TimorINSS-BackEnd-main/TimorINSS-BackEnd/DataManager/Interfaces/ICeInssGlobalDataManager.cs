using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICeInssGlobalDataManager
    {
        CeInssGlobalResponse GetReport(CeInssGlobalRequest request);
    }
}

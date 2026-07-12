using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICodigoContaOpeningBalanceDataManager
    {
        CodigoContaOpeningBalanceListResponse GetAll();
        ResponseBaseDataContract Update(UpdateCodigoContaOpeningBalanceRequest request);
    }
}

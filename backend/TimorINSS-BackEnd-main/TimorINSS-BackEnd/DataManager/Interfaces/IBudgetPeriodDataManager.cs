using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IBudgetPeriodDataManager
    {
        BudgetPeriodListResponse GetAll();
        ResponseBaseDataContract Save(SaveBudgetPeriodRequest request);
        ResponseBaseDataContract Deactivate(DeactivateBudgetPeriodRequest request);
    }
}

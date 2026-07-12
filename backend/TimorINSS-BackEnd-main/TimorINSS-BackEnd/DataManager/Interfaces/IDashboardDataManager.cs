using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDashboardDataManager
    {
        DashboardSummaryResponse GetSummary();
    }
}

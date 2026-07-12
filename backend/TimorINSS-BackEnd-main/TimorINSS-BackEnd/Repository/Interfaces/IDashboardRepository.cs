using TimorINSSBackEnd.DataContracts.ModelDataContract;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        DashboardSummaryDataContract GetSummary();
    }
}

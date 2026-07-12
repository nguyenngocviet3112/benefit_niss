using System;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DashboardDataManager : IDashboardDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DashboardSummaryResponse GetSummary()
        {
            DashboardSummaryResponse response = new DashboardSummaryResponse();
            try
            {
                response.Summary = _unitOfWork.DashboardRepository.GetSummary();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}

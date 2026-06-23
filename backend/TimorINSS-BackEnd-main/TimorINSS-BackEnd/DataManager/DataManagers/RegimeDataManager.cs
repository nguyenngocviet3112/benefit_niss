using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class RegimeDataManager : IRegimeDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public RegimeDataManager(IUnitOfWork unitOfWork,
                                IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }
    }
}
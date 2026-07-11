using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class UserModeAccessDataManager : IUserModeAccessDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserModeAccessDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool HasAccess(int utilizadorFk)
        {
            return _unitOfWork.UserModeAccessRepository.HasAccess(utilizadorFk);
        }
    }
}

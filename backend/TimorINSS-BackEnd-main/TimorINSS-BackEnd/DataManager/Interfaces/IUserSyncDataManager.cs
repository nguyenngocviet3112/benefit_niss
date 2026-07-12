using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IUserSyncDataManager
    {
        UserSyncListResponse GetInternalList();
        UserSyncListResponse GetExternalList();
        SyncInternalUserResponse SyncInternal(SyncInternalUserRequest request);
    }
}

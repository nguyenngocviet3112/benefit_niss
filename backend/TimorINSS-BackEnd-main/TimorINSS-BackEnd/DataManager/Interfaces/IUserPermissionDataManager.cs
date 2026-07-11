using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IUserPermissionDataManager
    {
        PermissionCatalogResponse GetCatalog();
        UserPermissionListResponse GetUsers();
        UserPermissionDetailResponse GetUser(GetUserPermissionRequest request);
        SaveUserPermissionResponse SaveUser(SaveUserPermissionRequest request);
    }
}

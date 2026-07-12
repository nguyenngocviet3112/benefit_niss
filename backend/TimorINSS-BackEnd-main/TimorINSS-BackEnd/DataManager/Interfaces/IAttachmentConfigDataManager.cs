using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IAttachmentConfigDataManager
    {
        AttachmentConfigResponse Get();
        ResponseBaseDataContract Save(SaveAttachmentConfigRequest request);
    }
}

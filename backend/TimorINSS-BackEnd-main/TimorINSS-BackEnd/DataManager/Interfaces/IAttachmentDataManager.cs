using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IAttachmentDataManager
    {
        AttachmentListResponse GetByEntity(GetAttachmentsByEntityRequest request);
        UploadAttachmentResponse Upload(UploadAttachmentRequest request);
        Attachment GetForDownload(int id);
    }
}

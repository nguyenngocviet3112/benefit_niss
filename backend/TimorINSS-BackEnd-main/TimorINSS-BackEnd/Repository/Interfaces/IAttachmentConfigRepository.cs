using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IAttachmentConfigRepository
    {
        AttachmentConfig Get();
        void Update(AttachmentConfig entity);
    }
}

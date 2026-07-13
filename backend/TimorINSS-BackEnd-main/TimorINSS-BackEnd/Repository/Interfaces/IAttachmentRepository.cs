using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IAttachmentRepository
    {
        List<Attachment> GetByEntity(string entityType, int entityId);
        Attachment Get(int id);
        bool ExistsForEntity(string entityType, int entityId);
        void Add(Attachment entity);
        void Update(Attachment entity);
    }
}

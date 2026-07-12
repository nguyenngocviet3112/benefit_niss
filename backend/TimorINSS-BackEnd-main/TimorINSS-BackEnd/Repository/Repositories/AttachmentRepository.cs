using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public AttachmentRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        // Metadata only — FileContent (varbinary(max)) deliberately left out of the
        // projection so listing attachments never pulls full file bytes off disk
        // (lesson learned from ComponenteDocumentosRegistoRepository, which does not).
        public List<Attachment> GetByEntity(string entityType, int entityId)
        {
            return _moduloContribuicoesContext.Attachment
                .Where(a => a.IndActivo && a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.DataCriacao)
                .Select(a => new Attachment
                {
                    Id = a.Id,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    DataCriacao = a.DataCriacao,
                    UtilizadorCriacao = a.UtilizadorCriacao
                })
                .ToList();
        }

        public Attachment Get(int id)
        {
            return _moduloContribuicoesContext.Attachment.SingleOrDefault(a => a.Id == id && a.IndActivo);
        }

        public bool ExistsForEntity(string entityType, int entityId)
        {
            return _moduloContribuicoesContext.Attachment.Any(a => a.IndActivo && a.EntityType == entityType && a.EntityId == entityId);
        }

        public void Add(Attachment entity)
        {
            _moduloContribuicoesContext.Attachment.Add(entity);
        }
    }
}

using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // Singleton config (Id always = 1) — max upload size + which stages require a
    // mandatory attachment before submit. Migration 2026-07-13c already seeds the
    // default row; GetOrCreateDefault is just a defensive fallback.
    public class AttachmentConfigRepository : IAttachmentConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public AttachmentConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public AttachmentConfig Get()
        {
            var config = _moduloContribuicoesContext.AttachmentConfig.SingleOrDefault(c => c.Id == 1);
            if (config == null)
            {
                config = new AttachmentConfig { Id = 1, MaxFileSizeMb = 10 };
                _moduloContribuicoesContext.AttachmentConfig.Add(config);
                _moduloContribuicoesContext.SaveChanges();
            }
            return config;
        }

        public void Update(AttachmentConfig entity)
        {
            AttachmentConfig entityToUpdate = _moduloContribuicoesContext.AttachmentConfig
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

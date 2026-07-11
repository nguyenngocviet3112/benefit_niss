using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class LanguageConfigRepository : ILanguageConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public LanguageConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<LanguageConfig> GetAll()
        {
            return _moduloContribuicoesContext.LanguageConfig
                .OrderBy(a => a.Ordem)
                .ToList();
        }

        public List<LanguageConfig> GetAllActive()
        {
            return _moduloContribuicoesContext.LanguageConfig
                .Where(a => a.IndActivo)
                .OrderBy(a => a.Ordem)
                .ToList();
        }

        public LanguageConfig Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.LanguageConfig.SingleOrDefault(a => a.Id == id);
        }

        public void Update(LanguageConfig entity)
        {
            LanguageConfig entityToUpdate = _moduloContribuicoesContext.LanguageConfig
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

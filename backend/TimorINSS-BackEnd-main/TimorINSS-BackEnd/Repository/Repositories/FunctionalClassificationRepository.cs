using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class FunctionalClassificationRepository : IFunctionalClassificationRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public FunctionalClassificationRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<FunctionalClassification> GetAllActive()
        {
            return _moduloContribuicoesContext.FunctionalClassification
                .Where(a => a.IndActivo)
                .OrderBy(a => a.Codigo)
                .ToList();
        }

        public FunctionalClassification Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.FunctionalClassification.SingleOrDefault(a => a.Id == id);
        }

        public void Add(FunctionalClassification entity)
        {
            _moduloContribuicoesContext.FunctionalClassification.Add(entity);
        }

        public void Update(FunctionalClassification entity)
        {
            FunctionalClassification entityToUpdate = _moduloContribuicoesContext.FunctionalClassification
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool IsCodeValid(FunctionalClassification entity)
        {
            int countSameCode = _moduloContribuicoesContext.FunctionalClassification
                .Where(a => a.IndActivo && a.Codigo == entity.Codigo && a.Id != entity.Id)
                .Count();

            return countSameCode == 0;
        }

        public bool HasActiveChildren(int id)
        {
            return _moduloContribuicoesContext.FunctionalClassification
                .Any(a => a.IndActivo && a.ParentFk == id);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class EconomicClassificationRepository : IEconomicClassificationRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public EconomicClassificationRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<EconomicClassification> GetTreeByOrcamentoConfig(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.EconomicClassification
                .Where(a => a.IndActivo && a.BudgetPeriodFk == orcamentoConfigFk)
                .OrderBy(a => a.Nivel)
                .ThenBy(a => a.Codigo)
                .ToList();
        }

        public EconomicClassification Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.EconomicClassification.SingleOrDefault(a => a.Id == id);
        }

        public void Add(EconomicClassification entity)
        {
            _moduloContribuicoesContext.EconomicClassification.Add(entity);
        }

        public void Update(EconomicClassification entity)
        {
            EconomicClassification entityToUpdate = _moduloContribuicoesContext.EconomicClassification
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool IsCodeValid(EconomicClassification entity)
        {
            int countSameCode = _moduloContribuicoesContext.EconomicClassification
                .Where(a => a.IndActivo
                    && a.BudgetPeriodFk == entity.BudgetPeriodFk
                    && a.Codigo == entity.Codigo
                    && a.Id != entity.Id)
                .Count();

            return countSameCode == 0;
        }

        public bool HasActiveChildren(int id)
        {
            return _moduloContribuicoesContext.EconomicClassification
                .Any(a => a.IndActivo && a.ParentFk == id);
        }
    }
}

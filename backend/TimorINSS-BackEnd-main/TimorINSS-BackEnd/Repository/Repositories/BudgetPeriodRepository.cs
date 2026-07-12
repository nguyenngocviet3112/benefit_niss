using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class BudgetPeriodRepository : IBudgetPeriodRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public BudgetPeriodRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<BudgetPeriod> GetAll()
        {
            return _moduloContribuicoesContext.BudgetPeriod.ToList();
        }

        public BudgetPeriod Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.BudgetPeriod.SingleOrDefault(u => u.Id == id);
        }

        public void Add(BudgetPeriod entity)
        {
            _moduloContribuicoesContext.BudgetPeriod.Add(entity);
        }

        public void Update(BudgetPeriod entity)
        {
            BudgetPeriod entityToUpdate = _moduloContribuicoesContext.BudgetPeriod
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool IsAnoTipoValid(BudgetPeriod period)
        {
            int count = _moduloContribuicoesContext.BudgetPeriod
                .Where(o => o.Id != period.Id && o.IndActivo
                    && o.Ano == period.Ano
                    && o.Tipo == period.Tipo)
                .Count();

            return count == 0;
        }

        public bool HasOrcamentoBatch(int id)
        {
            return _moduloContribuicoesContext.OrcamentoBatch
                .Any(b => b.BudgetPeriodFk == id && b.IndActivo);
        }

        public bool HasDependents(int id)
        {
            bool hasProgramActivity = _moduloContribuicoesContext.ProgramActivity
                .Any(a => a.BudgetPeriodFk == id && a.IndActivo);
            bool hasEconomicClassification = _moduloContribuicoesContext.EconomicClassification
                .Any(a => a.BudgetPeriodFk == id && a.IndActivo);
            bool hasOrcamentoSuplementar = _moduloContribuicoesContext.OrcamentoSuplementar
                .Any(b => b.BudgetPeriodFk == id && b.IndActivo);

            return hasProgramActivity || hasEconomicClassification || hasOrcamentoSuplementar;
        }
    }
}

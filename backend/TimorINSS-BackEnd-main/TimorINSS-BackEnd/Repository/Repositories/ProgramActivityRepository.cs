using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ProgramActivityRepository : IProgramActivityRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ProgramActivityRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<ProgramActivity> GetTreeByOrcamentoConfig(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.ProgramActivity
                .Where(a => a.IndActivo && a.OrcamentoConfigFk == orcamentoConfigFk)
                .OrderBy(a => a.Nivel)
                .ThenBy(a => a.Codigo)
                .ToList();
        }

        public ProgramActivity Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.ProgramActivity.SingleOrDefault(a => a.Id == id);
        }

        public void Add(ProgramActivity entity)
        {
            _moduloContribuicoesContext.ProgramActivity.Add(entity);
        }

        public void Update(ProgramActivity entity)
        {
            ProgramActivity entityToUpdate = _moduloContribuicoesContext.ProgramActivity
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool IsCodeValid(ProgramActivity entity)
        {
            int countSameCode = _moduloContribuicoesContext.ProgramActivity
                .Where(a => a.IndActivo
                    && a.OrcamentoConfigFk == entity.OrcamentoConfigFk
                    && a.Codigo == entity.Codigo
                    && a.Id != entity.Id)
                .Count();

            return countSameCode == 0;
        }

        public bool HasActiveChildren(int id)
        {
            return _moduloContribuicoesContext.ProgramActivity
                .Any(a => a.IndActivo && a.ParentFk == id);
        }
    }
}

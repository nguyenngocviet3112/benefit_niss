using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class OrcamentoSuplementarRepository : IOrcamentoSuplementarRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public OrcamentoSuplementarRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public OrcamentoSuplementar GetActiveDraftBatch(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.OrcamentoSuplementar
                .Where(b => b.IndActivo && b.OrcamentoConfigFk == orcamentoConfigFk && b.Estado != "APPROVED")
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();
        }

        public OrcamentoSuplementar Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.OrcamentoSuplementar.SingleOrDefault(b => b.Id == id);
        }

        public void Add(OrcamentoSuplementar entity)
        {
            _moduloContribuicoesContext.OrcamentoSuplementar.Add(entity);
        }

        public void Update(OrcamentoSuplementar entity)
        {
            OrcamentoSuplementar entityToUpdate = _moduloContribuicoesContext.OrcamentoSuplementar
                .Single(b => b.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public List<OrcamentoSuplementarLinha> GetLinhasByBatch(int batchId)
        {
            return _moduloContribuicoesContext.OrcamentoSuplementarLinha
                .Include(l => l.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(l => l.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(l => l.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation)
                .Where(l => l.IndActivo && l.OrcamentoSuplementarFk == batchId)
                .ToList();
        }

        public OrcamentoSuplementarLinha GetLinha(int id)
        {
            return _moduloContribuicoesContext.OrcamentoSuplementarLinha
                .Include(l => l.OrcamentoLinhaFkNavigation)
                .SingleOrDefault(l => l.Id == id);
        }

        public void AddLinha(OrcamentoSuplementarLinha entity)
        {
            _moduloContribuicoesContext.OrcamentoSuplementarLinha.Add(entity);
        }

        public void UpdateLinha(OrcamentoSuplementarLinha entity)
        {
            OrcamentoSuplementarLinha entityToUpdate = _moduloContribuicoesContext.OrcamentoSuplementarLinha
                .Single(l => l.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class OrcamentoBatchRepository : IOrcamentoBatchRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public OrcamentoBatchRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public OrcamentoBatch GetActiveDraftBatch(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.OrcamentoBatch
                .Where(b => b.IndActivo && b.OrcamentoConfigFk == orcamentoConfigFk && b.Estado != "APPROVED")
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();
        }

        public OrcamentoBatch GetLatestBatch(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.OrcamentoBatch
                .Where(b => b.IndActivo && b.OrcamentoConfigFk == orcamentoConfigFk)
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();
        }

        public OrcamentoBatch Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.OrcamentoBatch.SingleOrDefault(b => b.Id == id);
        }

        public void Add(OrcamentoBatch entity)
        {
            _moduloContribuicoesContext.OrcamentoBatch.Add(entity);
        }

        public void Update(OrcamentoBatch entity)
        {
            OrcamentoBatch entityToUpdate = _moduloContribuicoesContext.OrcamentoBatch
                .Single(b => b.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

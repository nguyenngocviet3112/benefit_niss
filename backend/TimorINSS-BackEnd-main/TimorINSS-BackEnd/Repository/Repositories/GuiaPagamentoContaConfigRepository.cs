using Microsoft.EntityFrameworkCore;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class GuiaPagamentoContaConfigRepository : IGuiaPagamentoContaConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public GuiaPagamentoContaConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        // Singleton config — no per-tenant/per-year variant needed (GP is always
        // "Receita de Contribuições Sociais"). Only ever one active row.
        public GuiaPagamentoContaConfig GetActive()
        {
            return _moduloContribuicoesContext.GuiaPagamentoContaConfig
                .Include(c => c.CodigoContaCreditoPrivadoFkNavigation)
                .Include(c => c.CodigoContaCreditoPublicoFkNavigation)
                .Where(c => c.IndActivo)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public void Add(GuiaPagamentoContaConfig entity)
        {
            _moduloContribuicoesContext.GuiaPagamentoContaConfig.Add(entity);
        }

        public void Update(GuiaPagamentoContaConfig entity)
        {
            GuiaPagamentoContaConfig entityToUpdate = _moduloContribuicoesContext.GuiaPagamentoContaConfig
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

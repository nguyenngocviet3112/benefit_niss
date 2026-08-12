using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ComponenteOrcamentoAjusteRepository : IComponenteOrcamentoAjusteRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ComponenteOrcamentoAjusteRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public ComponenteOrcamentoAjuste Get(int id)
        {
            return _moduloContribuicoesContext.ComponenteOrcamentoAjuste
                .SingleOrDefault(a => a.Id == id);
        }

        public void Add(ComponenteOrcamentoAjuste entity)
        {
            _moduloContribuicoesContext.ComponenteOrcamentoAjuste.Add(entity);
        }

        public void Update(ComponenteOrcamentoAjuste entity)
        {
            ComponenteOrcamentoAjuste entityToUpdate = _moduloContribuicoesContext.ComponenteOrcamentoAjuste
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public List<ComponenteOrcamentoAjuste> GetPendentesByComponenteOrcamentoRegistoFk(int componenteOrcamentoRegistoFk)
        {
            return _moduloContribuicoesContext.ComponenteOrcamentoAjuste
                .Where(a => a.IndActivo && a.ComponenteOrcamentoRegistoFk == componenteOrcamentoRegistoFk && a.Estado == "Pendente")
                .OrderByDescending(a => a.DataSolicitacao)
                .ToList();
        }

        public List<ComponenteOrcamentoAjuste> GetHistoricoByComponenteOrcamentoRegistoFk(int componenteOrcamentoRegistoFk)
        {
            return _moduloContribuicoesContext.ComponenteOrcamentoAjuste
                .Where(a => a.IndActivo && a.ComponenteOrcamentoRegistoFk == componenteOrcamentoRegistoFk)
                .OrderByDescending(a => a.DataSolicitacao)
                .ToList();
        }
    }
}

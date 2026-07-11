using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class CabimentoRepository : ICabimentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CabimentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<Cabimento> GetByAno(int ano)
        {
            return _moduloContribuicoesContext.Cabimento
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation)
                .Where(c => c.IndActivo && c.Ano == ano)
                .OrderBy(c => c.Mes).ThenBy(c => c.Numero)
                .ToList();
        }

        public Cabimento Get(int id)
        {
            return _moduloContribuicoesContext.Cabimento
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation)
                .SingleOrDefault(c => c.Id == id);
        }

        public void Add(Cabimento entity)
        {
            _moduloContribuicoesContext.Cabimento.Add(entity);
        }

        public void Update(Cabimento entity)
        {
            Cabimento entityToUpdate = _moduloContribuicoesContext.Cabimento
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.Cabimento
                .Where(c => c.IndActivo && c.Mes == mes && c.Ano == ano)
                .Select(c => (int?)c.Numero)
                .Max();

            return (max ?? 0) + 1;
        }

        public bool HasCabimentoForAd(int expenditureAuthorizationFk)
        {
            return _moduloContribuicoesContext.Cabimento
                .Any(c => c.IndActivo && c.ExpenditureAuthorizationFk == expenditureAuthorizationFk);
        }
    }
}

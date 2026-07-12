using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ExpenditureAuthorizationRepository : IExpenditureAuthorizationRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ExpenditureAuthorizationRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<ExpenditureAuthorization> GetByAno(int ano)
        {
            return _moduloContribuicoesContext.ExpenditureAuthorization
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation)
                .Include(a => a.ExpenditureAuthorizationPlurianualidade)
                .Where(a => a.IndActivo && a.Ano == ano)
                .OrderBy(a => a.Mes).ThenBy(a => a.Numero)
                .ToList();
        }

        public ExpenditureAuthorization Get(int id)
        {
            return _moduloContribuicoesContext.ExpenditureAuthorization
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation)
                .Include(a => a.ExpenditureAuthorizationPlurianualidade)
                .SingleOrDefault(a => a.Id == id);
        }

        public void Add(ExpenditureAuthorization entity)
        {
            _moduloContribuicoesContext.ExpenditureAuthorization.Add(entity);
        }

        public void Update(ExpenditureAuthorization entity)
        {
            ExpenditureAuthorization entityToUpdate = _moduloContribuicoesContext.ExpenditureAuthorization
                .Single(a => a.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.ExpenditureAuthorization
                .Where(a => a.IndActivo && a.Mes == mes && a.Ano == ano)
                .Select(a => (int?)a.Numero)
                .Max();

            return (max ?? 0) + 1;
        }

        public Dictionary<int, decimal> GetCabimentadoByAdIds(List<int> adIds)
        {
            return _moduloContribuicoesContext.Cabimento
                .Where(c => c.IndActivo && adIds.Contains(c.ExpenditureAuthorizationFk))
                .ToList()
                .GroupBy(c => c.ExpenditureAuthorizationFk)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.ValorCabimentado));
        }

        public bool HasAuthorizationForRubrica(int orcamentoLinhaFk)
        {
            return _moduloContribuicoesContext.ExpenditureAuthorization
                .Any(a => a.IndActivo && a.OrcamentoLinhaFk == orcamentoLinhaFk);
        }

        public ExpenditureAuthorization GetByRubrica(int orcamentoLinhaFk)
        {
            return _moduloContribuicoesContext.ExpenditureAuthorization
                .FirstOrDefault(a => a.IndActivo && a.OrcamentoLinhaFk == orcamentoLinhaFk);
        }

        public void AddPlurianualidade(ExpenditureAuthorizationPlurianualidade entity)
        {
            _moduloContribuicoesContext.ExpenditureAuthorizationPlurianualidade.Add(entity);
        }

        public void UpdatePlurianualidade(ExpenditureAuthorizationPlurianualidade entity)
        {
            ExpenditureAuthorizationPlurianualidade entityToUpdate = _moduloContribuicoesContext.ExpenditureAuthorizationPlurianualidade
                .Single(p => p.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public ExpenditureAuthorizationPlurianualidade GetPlurianualidade(int id)
        {
            return _moduloContribuicoesContext.ExpenditureAuthorizationPlurianualidade.SingleOrDefault(p => p.Id == id);
        }
    }
}

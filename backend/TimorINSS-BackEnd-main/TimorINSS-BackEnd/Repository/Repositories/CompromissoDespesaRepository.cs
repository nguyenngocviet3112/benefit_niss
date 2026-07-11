using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class CompromissoDespesaRepository : ICompromissoDespesaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CompromissoDespesaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<CompromissoDespesa> BaseQuery()
        {
            return _moduloContribuicoesContext.CompromissoDespesa
                .Include(c => c.CompromissoDespesaPlurianualidade)
                .Include(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.OrganizationFkNavigation);
        }

        public List<CompromissoDespesa> GetByAno(int ano)
        {
            return BaseQuery()
                .Where(c => c.IndActivo && c.Ano == ano)
                .OrderBy(c => c.Mes).ThenBy(c => c.Numero)
                .ToList();
        }

        public CompromissoDespesa Get(int id)
        {
            return BaseQuery().SingleOrDefault(c => c.Id == id);
        }

        public void Add(CompromissoDespesa entity)
        {
            _moduloContribuicoesContext.CompromissoDespesa.Add(entity);
        }

        public void Update(CompromissoDespesa entity)
        {
            CompromissoDespesa entityToUpdate = _moduloContribuicoesContext.CompromissoDespesa
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.CompromissoDespesa
                .Where(c => c.IndActivo && c.Mes == mes && c.Ano == ano)
                .Select(c => (int?)c.Numero)
                .Max();

            return (max ?? 0) + 1;
        }

        public void AddPlurianualidade(CompromissoDespesaPlurianualidade entity)
        {
            _moduloContribuicoesContext.CompromissoDespesaPlurianualidade.Add(entity);
        }

        public void UpdatePlurianualidade(CompromissoDespesaPlurianualidade entity)
        {
            CompromissoDespesaPlurianualidade entityToUpdate = _moduloContribuicoesContext.CompromissoDespesaPlurianualidade
                .Single(p => p.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public CompromissoDespesaPlurianualidade GetPlurianualidade(int id)
        {
            return _moduloContribuicoesContext.CompromissoDespesaPlurianualidade.SingleOrDefault(p => p.Id == id);
        }
    }
}

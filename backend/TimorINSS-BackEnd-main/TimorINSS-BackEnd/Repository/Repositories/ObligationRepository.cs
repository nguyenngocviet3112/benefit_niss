using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ObligationRepository : IObligationRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ObligationRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<Obligation> BaseQuery()
        {
            return _moduloContribuicoesContext.Obligation
                .Include(o => o.ObligationItem).ThenInclude(i => i.CompromissoDespesaFkNavigation).ThenInclude(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(o => o.ObligationItem).ThenInclude(i => i.CompromissoDespesaFkNavigation).ThenInclude(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation);
        }

        public List<Obligation> GetByAno(int ano)
        {
            return BaseQuery()
                .Where(o => o.IndActivo && o.Ano == ano)
                .OrderBy(o => o.Mes).ThenBy(o => o.Numero)
                .ToList();
        }

        public Obligation Get(int id)
        {
            return BaseQuery().SingleOrDefault(o => o.Id == id);
        }

        public void Add(Obligation entity)
        {
            _moduloContribuicoesContext.Obligation.Add(entity);
        }

        public void Update(Obligation entity)
        {
            Obligation entityToUpdate = _moduloContribuicoesContext.Obligation
                .Single(o => o.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.Obligation
                .Where(o => o.IndActivo && o.Mes == mes && o.Ano == ano)
                .Select(o => (int?)o.Numero)
                .Max();

            return (max ?? 0) + 1;
        }

        public void AddItem(ObligationItem entity)
        {
            _moduloContribuicoesContext.ObligationItem.Add(entity);
        }

        public void UpdateItem(ObligationItem entity)
        {
            ObligationItem entityToUpdate = _moduloContribuicoesContext.ObligationItem
                .Single(i => i.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public ObligationItem GetItem(int id)
        {
            return _moduloContribuicoesContext.ObligationItem.SingleOrDefault(i => i.Id == id);
        }

        public decimal GetTotalCommittedForCompromisso(int compromissoDespesaFk)
        {
            return _moduloContribuicoesContext.ObligationItem
                .Where(i => i.IndActivo && i.CompromissoDespesaFk == compromissoDespesaFk)
                .Select(i => (decimal?)i.Value)
                .Sum() ?? 0;
        }
    }
}

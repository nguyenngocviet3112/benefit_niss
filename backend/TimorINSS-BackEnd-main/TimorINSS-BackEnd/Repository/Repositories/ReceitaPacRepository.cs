using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ReceitaPacRepository : IReceitaPacRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ReceitaPacRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<ReceitaPac> BaseQuery()
        {
            return _moduloContribuicoesContext.ReceitaPac
                .Include(r => r.RegimeFkNavigation)
                .Include(r => r.AtividadeFkNavigation)
                .Include(r => r.EconomicClassificationFkNavigation)
                .Include(r => r.OrganizationFkNavigation);
        }

        public List<ReceitaPac> GetByAno(int ano)
        {
            return BaseQuery()
                .Where(r => r.IndActivo && r.Ano == ano)
                .OrderBy(r => r.Mes).ThenBy(r => r.Numero)
                .ToList();
        }

        public ReceitaPac Get(int id)
        {
            return BaseQuery().SingleOrDefault(r => r.Id == id);
        }

        public void Add(ReceitaPac entity)
        {
            _moduloContribuicoesContext.ReceitaPac.Add(entity);
        }

        public void Update(ReceitaPac entity)
        {
            ReceitaPac entityToUpdate = _moduloContribuicoesContext.ReceitaPac
                .Single(r => r.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.ReceitaPac
                .Where(r => r.IndActivo && r.Mes == mes && r.Ano == ano)
                .Select(r => (int?)r.Numero)
                .Max();

            return (max ?? 0) + 1;
        }
    }
}

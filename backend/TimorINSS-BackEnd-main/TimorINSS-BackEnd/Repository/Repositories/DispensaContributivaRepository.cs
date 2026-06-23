using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class DispensaContributivaRepository : IDispensaContributivaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public DispensaContributivaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Dispensacontributiva> GetAll()
        {
            return _moduloContribuicoesContext.Dispensacontributiva.Where(u => u.IndActivo).ToList();
        }

        public Dispensacontributiva Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var dispensaContributiva = _moduloContribuicoesContext.Dispensacontributiva
                .SingleOrDefault(u => u.IdDispContributiva == id);

            return dispensaContributiva;
        }

        public DispensaContributivaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var dispensaContributiva = _moduloContribuicoesContext.Dispensacontributiva
                .SingleOrDefault(u => u.IdDispContributiva == id);

            DispensaContributivaDto dispensaContributivaDto = Utils.MappClassToDto<Dispensacontributiva, DispensaContributivaDto>(dispensaContributiva);
            return dispensaContributivaDto;
        }

        public void Add(Dispensacontributiva entity)
        {
            _moduloContribuicoesContext.Dispensacontributiva.Add(entity);
        }

        public void Update(Dispensacontributiva entity)
        {
            Dispensacontributiva entityToUpdate = _moduloContribuicoesContext.Dispensacontributiva
                .Single(d => d.IdDispContributiva == entity.IdDispContributiva);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Dispensacontributiva entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public decimal getDispensaContributivaByYear(int year)
        {
            decimal response = _moduloContribuicoesContext.Dispensacontributiva
                .Where(d => d.IndActivo && d.Ano == year)
                .Select(d => d.Percentagem).FirstOrDefault();

            return response;
        }
    }
}
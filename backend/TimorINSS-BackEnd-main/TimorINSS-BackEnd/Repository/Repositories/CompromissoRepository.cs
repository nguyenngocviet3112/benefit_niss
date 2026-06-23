using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;
using System.Linq;


namespace TimorINSSBackEnd.Repository.Repositories
{
    public class CompromissoRepository : ICompromissoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CompromissoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Compromisso> GetAll()
        {
            return _moduloContribuicoesContext.Compromisso.Where(u => u.IndActivo).ToList();
        }

        public Compromisso Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var compromisso = _moduloContribuicoesContext.Compromisso
                .SingleOrDefault(u => u.Id == id);

            return compromisso;
        }

        public CompromissoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var compromisso = _moduloContribuicoesContext.Compromisso
                .SingleOrDefault(u => u.Id == id);

            CompromissoDto compromissoDto = Utils.MappClassToDto<Compromisso, CompromissoDto>(compromisso);
            return compromissoDto;
        }

        public void Add(Compromisso entity)
        {
            _moduloContribuicoesContext.Compromisso.Add(entity);
        }

        public void Update(Compromisso entity)
        {
            Compromisso entityToUpdate = _moduloContribuicoesContext.Compromisso
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Compromisso entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }
    }
}

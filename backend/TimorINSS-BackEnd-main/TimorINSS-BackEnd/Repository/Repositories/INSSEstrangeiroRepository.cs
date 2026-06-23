using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class INSSEstrangeiroRepository : IINSSEstrangeiroRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public INSSEstrangeiroRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Inssestrangeiro> GetAll()
        {
            return _moduloContribuicoesContext.Inssestrangeiro.Where(u => u.IndActivo).ToList();
        }

        public Inssestrangeiro Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var Inssestrangeiro = _moduloContribuicoesContext.Inssestrangeiro
                .SingleOrDefault(u => u.IdInssestrang == id);

            return Inssestrangeiro;
        }

        public InssestrangeiroDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var Inssestrangeiro = _moduloContribuicoesContext.Inssestrangeiro
                .SingleOrDefault(u => u.IdInssestrang == id);

            InssestrangeiroDto InssestrangeiroDto = Utils.MappClassToDto<Inssestrangeiro, InssestrangeiroDto>(Inssestrangeiro);
            return InssestrangeiroDto;
        }

        public void Add(Inssestrangeiro entity)
        {
            _moduloContribuicoesContext.Inssestrangeiro.Add(entity);
        }

        public void Update(Inssestrangeiro entity)
        {
            Inssestrangeiro entityToUpdate = _moduloContribuicoesContext.Inssestrangeiro
                .Single(d => d.IdInssestrang == entity.IdInssestrang);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Inssestrangeiro entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }
    }
}
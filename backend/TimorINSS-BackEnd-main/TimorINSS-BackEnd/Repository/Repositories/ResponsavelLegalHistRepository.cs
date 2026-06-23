using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ResponsavelLegalHistRepository : IResponsavelLegalHistRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ResponsavelLegalHistRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Responsavellegalhist> GetAll()
        {
            return _moduloContribuicoesContext.Responsavellegalhist.ToList();
        }

        public Responsavellegalhist Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var responsavellegalhist = _moduloContribuicoesContext.Responsavellegalhist
                .SingleOrDefault(u => u.IdResPlegalHist == id);

            return responsavellegalhist;
        }

        public ResponsavellegalhistDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var responsavellegalhist = _moduloContribuicoesContext.Responsavellegalhist
                .SingleOrDefault(u => u.IdResPlegalHist == id);

            ResponsavellegalhistDto responsavellegalhistDto = Utils.MappClassToDto<Responsavellegalhist, ResponsavellegalhistDto>(responsavellegalhist);
            return responsavellegalhistDto;
        }

        public void Add(Responsavellegalhist entity)
        {
            _moduloContribuicoesContext.Responsavellegalhist.Add(entity);
        }

        public void Update(Responsavellegalhist entity)
        {
            Responsavellegalhist entityToUpdate = _moduloContribuicoesContext.Responsavellegalhist
                .Single(d => d.IdResPlegalHist == entity.IdResPlegalHist);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Responsavellegalhist entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }
    }
}
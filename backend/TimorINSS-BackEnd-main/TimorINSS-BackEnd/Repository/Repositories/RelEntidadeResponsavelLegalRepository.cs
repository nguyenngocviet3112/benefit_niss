using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelEntidadeResponsavelLegalRepository : IRelEntidadeResponsavelLegalRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelEntidadeResponsavelLegalRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relentidaderesplegal> GetAll()
        {
            return _moduloContribuicoesContext.Relentidaderesplegal
                .ToList();
        }

        public Relentidaderesplegal Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var declaracaoremuneracao = _moduloContribuicoesContext.Relentidaderesplegal
                .SingleOrDefault(d => d.IdRel == id);

            return declaracaoremuneracao;
        }

        public RelentidaderesplegalDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relentidaderesplegal = _moduloContribuicoesContext.Relentidaderesplegal
                .SingleOrDefault(d => d.IdRel == id);

            RelentidaderesplegalDto relentidaderesplegalDto = Utils.MappClassToDto<Relentidaderesplegal, RelentidaderesplegalDto>(relentidaderesplegal);
            return relentidaderesplegalDto;
        }

        public void Add(Relentidaderesplegal entity)
        {
            _moduloContribuicoesContext.Relentidaderesplegal.Add(entity);
        }

        public void Update(Relentidaderesplegal entity)
        {
            Relentidaderesplegal entityToUpdate = _moduloContribuicoesContext.Relentidaderesplegal
                .Single(d => d.IdRel == entity.IdRel);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relentidaderesplegal entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public RelentidaderesplegalDto GetDtoByResponsavelLegal(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relentidaderesplegal = _moduloContribuicoesContext.Relentidaderesplegal
                .SingleOrDefault(d => d.RelRespLegalEntFk == id);

            RelentidaderesplegalDto relentidaderesplegalDto = Utils.MappClassToDto<Relentidaderesplegal, RelentidaderesplegalDto>(relentidaderesplegal);
            return relentidaderesplegalDto;
        }

        public Relentidaderesplegal GetByResponsavelLegal(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var declaracaoremuneracao = _moduloContribuicoesContext.Relentidaderesplegal
                .SingleOrDefault(d => d.RelRespLegalEntFk == id);

            return declaracaoremuneracao;
        }
    }
}
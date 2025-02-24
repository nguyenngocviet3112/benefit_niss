using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class CamposEditaveisRepository : ICamposEditaveisRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CamposEditaveisRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Camposeditaveis> GetAll()
        {
            return _moduloContribuicoesContext.Camposeditaveis
                .ToList();
        }

        public Camposeditaveis Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var campo = _moduloContribuicoesContext.Camposeditaveis
                .SingleOrDefault(u => u.IdCampoEditavel == id);

            return campo;
        }

        public CamposEditaveisDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var campo = _moduloContribuicoesContext.Camposeditaveis
                .SingleOrDefault(u => u.IdCampoEditavel == id);

            CamposEditaveisDto campoDto = Utils.MappClassToDto<Camposeditaveis, CamposEditaveisDto>(campo);
            return campoDto;
        }

        public void Add(Camposeditaveis entity)
        {
            _moduloContribuicoesContext.Camposeditaveis.Add(entity);
        }

        public void Update(Camposeditaveis entity)
        {
            Camposeditaveis entityToUpdate = _moduloContribuicoesContext.Camposeditaveis
                .Single(d => d.IdCampoEditavel == entity.IdCampoEditavel);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Camposeditaveis entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<CamposEditaveisListagem> getAllCamposEditaveis()
        {
            return _moduloContribuicoesContext.Camposeditaveis
                .Select(c => new CamposEditaveisListagem
                {
                    IdCampoEditavel = c.IdCampoEditavel,
                    Nome = c.Nome,
                    DominioFk = c.DominioFk,
                    CampoPaiFk = c.CampoPaiFk
                })
                .ToList();
        }
    }
}
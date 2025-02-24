using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class UtilizadorTokenRepository : IUtilizadorTokenRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public UtilizadorTokenRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Utilizadortoken> GetAll()
        {
            return _moduloContribuicoesContext.Utilizadortoken
                .ToList();
        }

        public Utilizadortoken Get(long id)
        {
            var utilizador = _moduloContribuicoesContext.Utilizadortoken
                .SingleOrDefault(u => u.Id == id);

            return utilizador;
        }

        public UtilizadortokenDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;
            var utilizadorToken = _moduloContribuicoesContext.Utilizadortoken
                .SingleOrDefault(u => u.Id == id);

            UtilizadortokenDto utilizadorTokenDto = Utils.MappClassToDto<Utilizadortoken, UtilizadortokenDto>(utilizadorToken);
            return utilizadorTokenDto;
        }

        public void Add(Utilizadortoken entity)
        {
            _moduloContribuicoesContext.Utilizadortoken.Add(entity);
        }

        public void Update(Utilizadortoken entity)
        {
            Utilizadortoken entityToUpdate = _moduloContribuicoesContext.Utilizadortoken
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Utilizadortoken entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<Utilizadortoken> GetByUserId(long userId)
        {
            var utilizador = _moduloContribuicoesContext.Utilizadortoken
                .Where(u => u.IdUtilizador == userId && u.IsRecover == null && u.EntidadeId == null)
                .ToList();

            return utilizador;
        }

        public List<Utilizadortoken> GetUniqueTokensByUserId(long userId)
        {
            var utilizador = _moduloContribuicoesContext.Utilizadortoken
                .Where(u => u.IdUtilizador == userId && u.IsRecover.Value)
                .ToList();

            return utilizador;
        }

        public List<Utilizadortoken> GetUniqueTokensByEntidadeId(int entidadeId)
        {
            var token = _moduloContribuicoesContext.Utilizadortoken
                .Where(u => u.EntidadeId == entidadeId)
                .ToList();

            return token;
        }
    }
}
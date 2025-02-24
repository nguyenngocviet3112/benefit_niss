using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public PerfilRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Perfil> GetAll()
        {
            return _moduloContribuicoesContext.Perfil.ToList();
        }

        public Perfil Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var perfil = _moduloContribuicoesContext.Perfil
                .SingleOrDefault(u => u.Id == id);

            return perfil;
        }

        public PerfilDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var perfil = _moduloContribuicoesContext.Perfil
                .SingleOrDefault(u => u.Id == id);

            PerfilDto perfilDto = Utils.MappClassToDto<Perfil, PerfilDto>(perfil);
            return perfilDto;
        }

        public void Add(Perfil entity)
        {
            _moduloContribuicoesContext.Perfil.Add(entity);
        }

        public void Update(Perfil entity)
        {
            Perfil entityToUpdate = _moduloContribuicoesContext.Perfil
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Perfil entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public PerfilListagemResponse GetAllPerfis(SearchFilterRequest request)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            var listaPerfil = _moduloContribuicoesContext.Perfil
               .Where(u => u.Descricao.Contains(request.filter.filterBy))
               .Select(u => new PerfilDataContract
               {
                   id = u.Id,
                   dataCriacao = u.DataCriacao,
                   descricao = u.Descricao,
                   indActivo = u.IndActivo
               });

            var perfis = listaPerfil
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = listaPerfil.Count();
            response.rows = totalNumber;
            response.perfil = perfis;

            return response;
        }

        public List<SelectDescription> GetAllPerfisAtivo()
        {
            return _moduloContribuicoesContext.Perfil
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Descricao,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public List<Perfil> GetPerfisByIds(List<int> perfilIds)
        {
            return _moduloContribuicoesContext.Perfil
                .Where(u => perfilIds.Contains(u.Id))
                .ToList();
        }

        public List<int> GetActivePerfisByIds(List<int> perfilIds)
        {
            return _moduloContribuicoesContext.Perfil
                .Where(u => perfilIds.Contains(u.Id) && u.IndActivo)
                .Select(x => x.Id)
                .ToList();
        }
    }
}
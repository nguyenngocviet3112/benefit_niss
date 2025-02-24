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
    public class PaisRepository : IPaisRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public PaisRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Pais> GetAll()
        {
            return _moduloContribuicoesContext.Pais.ToList();
        }

        public Pais Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var pais = _moduloContribuicoesContext.Pais
                .SingleOrDefault(u => u.IdPais == id);

            return pais;
        }

        public PaisDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var pais = _moduloContribuicoesContext.Pais
                .SingleOrDefault(u => u.IdPais == id);

            PaisDto paisDto = Utils.MappClassToDto<Pais, PaisDto>(pais);
            return paisDto;
        }

        public void Add(Pais entity)
        {
            _moduloContribuicoesContext.Pais.Add(entity);
        }

        public void Update(Pais entity)
        {
            Pais entityToUpdate = _moduloContribuicoesContext.Pais
                .Single(d => d.IdPais == entity.IdPais);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Pais entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> getAllPais()
        {
            return _moduloContribuicoesContext.Pais
               .Select(u => new SelectDescription
               {
                   id = u.IdPais,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse getAllActivePais(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryPais = _moduloContribuicoesContext.Pais
                .Where(p => p.IndActivo && p.Nome.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdPais,
                   Nome = u.Nome,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var paises = queryPais
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryPais.Count();

            response.ValuesCampo = paises;
            response.CountValuesCampo = totalNumber;

            return response;
        }
    }
}
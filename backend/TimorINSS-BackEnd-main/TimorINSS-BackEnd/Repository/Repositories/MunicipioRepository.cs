using Microsoft.EntityFrameworkCore;
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
    public class MunicipioRepository : IMunicipioRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public MunicipioRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Municipio> GetAll()
        {
            return _moduloContribuicoesContext.Municipio.ToList();
        }

        public Municipio Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var municipio = _moduloContribuicoesContext.Municipio
                .SingleOrDefault(u => u.IdMunicipio == id);

            return municipio;
        }

        public MunicipioDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var municipio = _moduloContribuicoesContext.Municipio
                .SingleOrDefault(u => u.IdMunicipio == id);

            MunicipioDto municipioDto = Utils.MappClassToDto<Municipio, MunicipioDto>(municipio);
            return municipioDto;
        }

        public void Add(Municipio entity)
        {
            _moduloContribuicoesContext.Municipio.Add(entity);
        }

        public void Update(Municipio entity)
        {
            Municipio entityToUpdate = _moduloContribuicoesContext.Municipio
                .Single(d => d.IdMunicipio == entity.IdMunicipio);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Municipio entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> getAllMunicipio()
        {
            return _moduloContribuicoesContext.Municipio
               .Select(u => new SelectDescription
               {
                   id = u.IdMunicipio,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse getAllActiveMunicipio(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryMunicipio = _moduloContribuicoesContext.Municipio
                .Where(m => m.IndActivo && m.Nome.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdMunicipio,
                   Nome = u.Nome,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var municipios = queryMunicipio
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryMunicipio.Count();

            response.ValuesCampo = municipios;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public Municipio GetWithActiveChilds(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var municipio = _moduloContribuicoesContext.Municipio
                .Include(u => u.Postoadministrativo.Where(p => p.IndActivo))
                .SingleOrDefault(u => u.IdMunicipio == id);

            return municipio;
        }

        public List<SelectDescription> getAllActiveMunicipio()
        {
            return _moduloContribuicoesContext.Municipio
                .Where(u => u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.IdMunicipio,
                    nome = u.Nome,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}
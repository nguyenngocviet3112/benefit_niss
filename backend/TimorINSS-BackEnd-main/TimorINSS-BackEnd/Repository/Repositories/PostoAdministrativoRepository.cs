using Microsoft.EntityFrameworkCore;
using System;
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
    public class PostoAdministrativoRepository : IPostoAdministrativoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public PostoAdministrativoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Postoadministrativo> GetAll()
        {
            return _moduloContribuicoesContext.Postoadministrativo.ToList();
        }

        public Postoadministrativo Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var postoAdministrativo = _moduloContribuicoesContext.Postoadministrativo
                .SingleOrDefault(u => u.IdPostoAdmin == id);

            return postoAdministrativo;
        }

        public PostoadministrativoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var postoAdministrativo = _moduloContribuicoesContext.Postoadministrativo
                .SingleOrDefault(u => u.IdPostoAdmin == id);

            PostoadministrativoDto postoAdministrativoDto = Utils.MappClassToDto<Postoadministrativo, PostoadministrativoDto>(postoAdministrativo);
            return postoAdministrativoDto;
        }

        public void Add(Postoadministrativo entity)
        {
            _moduloContribuicoesContext.Postoadministrativo.Add(entity);
        }

        public void Update(Postoadministrativo entity)
        {
            Postoadministrativo entityToUpdate = _moduloContribuicoesContext.Postoadministrativo
                .Single(d => d.IdPostoAdmin == entity.IdPostoAdmin);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Postoadministrativo entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> getAllPostoAdministrativo()
        {
            return _moduloContribuicoesContext.Postoadministrativo
               .Select(u => new SelectDescription
               {
                   id = u.IdPostoAdmin,
                   nome = u.Nome,
                   parentId = u.PostoAdminMunicipioFk,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public List<SelectDescription> getPostoByIdMunicipio(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            List<SelectDescription> posto = _moduloContribuicoesContext.Municipio
                .Where(u => u.IdMunicipio == id)
                .Join(
                    _moduloContribuicoesContext.Postoadministrativo,
                    municipio => municipio.IdMunicipio,
                    postoadministrativo => postoadministrativo.PostoAdminMunicipioFk,
                    (municipio, postoadministrativo) => new SelectDescription
                    {
                        id = postoadministrativo.IdPostoAdmin,
                        nome = postoadministrativo.Nome,
                        parentId = postoadministrativo.PostoAdminMunicipioFk,
                        indActivo = postoadministrativo.IndActivo
                    }
                )
                .ToList();
            return posto;
        }

        public ValueCampoEditavelListagemResponse getAllActivePostoAdministrativo(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Postoadministrativo> queryPostoAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryPostoAux = _moduloContribuicoesContext.Postoadministrativo
                    .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy) && a.PostoAdminMunicipioFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var queryPosto = queryPostoAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdPostoAdmin,
                   Nome = u.Nome,
                   ParentId = u.PostoAdminMunicipioFk,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var postos = queryPosto
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryPosto.Count();

            response.ValuesCampo = postos;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public Postoadministrativo GetWithActiveChilds(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var postoAdministrativo = _moduloContribuicoesContext.Postoadministrativo
                .Include(u => u.Suco.Where(s => s.IndActivo))
                .SingleOrDefault(u => u.IdPostoAdmin == id);

            return postoAdministrativo;
        }

        public List<SelectDescription> getAllActivePostoAdministrativo()
        {
            return _moduloContribuicoesContext.Postoadministrativo
                .Where(u => u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.IdPostoAdmin,
                    nome = u.Nome,
                    parentId = u.PostoAdminMunicipioFk,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}
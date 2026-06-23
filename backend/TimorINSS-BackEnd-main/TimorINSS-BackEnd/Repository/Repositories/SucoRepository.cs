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
    public class SucoRepository : ISucoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public SucoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Suco> GetAll()
        {
            return _moduloContribuicoesContext.Suco.ToList();
        }

        public Suco Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var suco = _moduloContribuicoesContext.Suco
                .SingleOrDefault(u => u.IdSuco == id);

            return suco;
        }

        public SucoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var suco = _moduloContribuicoesContext.Suco
                .SingleOrDefault(u => u.IdSuco == id);

            SucoDto sucoDto = Utils.MappClassToDto<Suco, SucoDto>(suco);
            return sucoDto;
        }

        public void Add(Suco entity)
        {
            _moduloContribuicoesContext.Suco.Add(entity);
        }

        public void Update(Suco entity)
        {
            Suco entityToUpdate = _moduloContribuicoesContext.Suco
                .Single(d => d.IdSuco == entity.IdSuco);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Suco entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> getAllSuco()
        {
            return _moduloContribuicoesContext.Suco
               .Select(u => new SelectDescription
               {
                   id = u.IdSuco,
                   nome = u.Nome,
                   parentId = u.SucoPostoAdminFk,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public List<SelectDescription> getSucoByIdPosto(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            List<SelectDescription> suco = _moduloContribuicoesContext.Postoadministrativo
                .Where(u => u.IdPostoAdmin == id)
                .Join(
                    _moduloContribuicoesContext.Suco,
                    postoadministrativo => postoadministrativo.IdPostoAdmin,
                    suco => suco.SucoPostoAdminFk,
                    (postoadministrativo, suco) => new SelectDescription
                    {
                        id = suco.IdSuco,
                        nome = suco.Nome,
                        parentId = suco.SucoPostoAdminFk,
                        indActivo = postoadministrativo.IndActivo
                    }
                )
                .ToList();
            return suco;
        }

        public ValueCampoEditavelListagemResponse getAllActiveSuco(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Suco> querySucoAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            if (int.TryParse(filter.filterField, out int parent))
                querySucoAux = _moduloContribuicoesContext.Suco
                    .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy) && a.SucoPostoAdminFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var querySuco = querySucoAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdSuco,
                   Nome = u.Nome,
                   ParentId = u.SucoPostoAdminFk,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var sucos = querySuco
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySuco.Count();

            response.ValuesCampo = sucos;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public Suco GetWithActiveChilds(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var suco = _moduloContribuicoesContext.Suco
                .Include(u => u.Aldeia.Where(a => a.IndActivo))
                .SingleOrDefault(u => u.IdSuco == id);

            return suco;
        }

        public List<SelectDescription> getAllActiveSuco()
        {
            return _moduloContribuicoesContext.Suco
                .Where(u => u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.IdSuco,
                    nome = u.Nome,
                    parentId = u.SucoPostoAdminFk,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}
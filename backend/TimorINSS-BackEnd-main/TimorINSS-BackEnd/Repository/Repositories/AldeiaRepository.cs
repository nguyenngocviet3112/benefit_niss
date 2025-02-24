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
    public class AldeiaRepository : IAldeiaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public AldeiaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Aldeia> GetAll()
        {
            return _moduloContribuicoesContext.Aldeia.ToList();
        }

        public Aldeia Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var aldeia = _moduloContribuicoesContext.Aldeia
                .SingleOrDefault(u => u.IdAldeia == id);

            return aldeia;
        }

        public AldeiaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var aldeia = _moduloContribuicoesContext.Aldeia
                .SingleOrDefault(u => u.IdAldeia == id);

            AldeiaDto aldeiaDto = Utils.MappClassToDto<Aldeia, AldeiaDto>(aldeia);
            return aldeiaDto;
        }

        public void Add(Aldeia entity)
        {
            _moduloContribuicoesContext.Aldeia.Add(entity);
        }

        public void Update(Aldeia entity)
        {
            Aldeia entityToUpdate = _moduloContribuicoesContext.Aldeia
                .Single(d => d.IdAldeia == entity.IdAldeia);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Aldeia entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> getAllAldeia()
        {
            return _moduloContribuicoesContext.Aldeia
               .Select(u => new SelectDescription
               {
                   id = u.IdAldeia,
                   nome = u.Nome,
                   parentId = u.AldeiaSucoFk,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public List<SelectDescription> getAldeiaByIdSuco(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            List<SelectDescription> aldeia = _moduloContribuicoesContext.Suco
                .Where(u => u.IdSuco == id)
                .Join(
                    _moduloContribuicoesContext.Aldeia,
                    suco => suco.IdSuco,
                    aldeia => aldeia.AldeiaSucoFk,
                    (suco, aldeia) => new SelectDescription
                    {
                        id = aldeia.IdAldeia,
                        nome = aldeia.Nome,
                        indActivo = aldeia.IndActivo
                    }
                )
                .ToList();
            return aldeia;
        }

        public ValueCampoEditavelListagemResponse getAllActiveAldeia(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Aldeia> queryAldeiasAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryAldeiasAux = _moduloContribuicoesContext.Aldeia
                    .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy) && a.AldeiaSucoFk == parent);
            else
                queryAldeiasAux = _moduloContribuicoesContext.Aldeia
                    .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy));

            var queryAldeias = queryAldeiasAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdAldeia,
                   Nome = u.Nome,
                   ParentId = u.AldeiaSucoFk,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var aldeias = queryAldeias
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryAldeias.Count();

            response.ValuesCampo = aldeias;
            response.CountValuesCampo = totalNumber;

            return response;
        }
    }
}
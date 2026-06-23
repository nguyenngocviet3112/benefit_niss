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
    public class RelTipoDeContaOrcamentoConfigRepository : IRelTipoDeContaOrcamentoConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelTipoDeContaOrcamentoConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Reltipodecontaorcamentoconfig> GetAll()
        {
            return _moduloContribuicoesContext.Reltipodecontaorcamentoconfig.ToList();
        }

        public Reltipodecontaorcamentoconfig Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relTipoDeContaOrcamentoConfig = _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                .SingleOrDefault(u => u.Id == id);

            return relTipoDeContaOrcamentoConfig;
        }

        public RelTipoDeContaOrcamentoConfigDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var relTipoDeContaOrcamentoConfig = _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                .SingleOrDefault(u => u.Id == id);

            RelTipoDeContaOrcamentoConfigDto relTipoDeContaOrcamentoConfigDto = Utils.MappClassToDto<Reltipodecontaorcamentoconfig, RelTipoDeContaOrcamentoConfigDto>(relTipoDeContaOrcamentoConfig);
            return relTipoDeContaOrcamentoConfigDto;
        }

        public void Add(Reltipodecontaorcamentoconfig entity)
        {
            _moduloContribuicoesContext.Reltipodecontaorcamentoconfig.Add(entity);
        }

        public void Update(Reltipodecontaorcamentoconfig entity)
        {
            Reltipodecontaorcamentoconfig entityToUpdate = _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Reltipodecontaorcamentoconfig entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllActiveRelTipoDeContaOrcamentoConfig(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Reltipodecontaorcamentoconfig> queryRelAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            if (int.TryParse(filter.filterField, out int parent))
                queryRelAux = _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                    .Include(r => r.TipoContaFkNavigation)
                    .Where(a => a.IndActivo &&
                                a.TipoContaFkNavigation.Descricao.Contains(filter.filterBy) &&
                                a.OrcamentoConfigFk == parent
                    );
            else
                throw new Exception("No id was found for the entity parent");

            var queryRel = queryRelAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.TipoContaFkNavigation.Descricao,
                   ParentId = u.OrcamentoConfigFk
               });

            var rels = queryRel
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryRel.Count();

            response.ValuesCampo = rels;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public List<SelectDescription> GetAllRelTipoDeContaOrcamentoConfigByParent(int parent)
        {
            int? parentOfParent = _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                .Where(u => u.Id == parent).Select(u => u.OrcamentoConfigFk).FirstOrDefault();

            if (!parentOfParent.HasValue)
                throw new Exception("No id was found for the entity parent of parent");

            return _moduloContribuicoesContext.Reltipodecontaorcamentoconfig
                .Include(r => r.TipoContaFkNavigation)
                .Where(u => u.IndActivo && u.OrcamentoConfigFk == parentOfParent.Value)
                .Select(u => new SelectDescription
                {
                    id = u.Id,
                    nome = u.TipoContaFkNavigation.Descricao,
                    parentId = u.OrcamentoConfigFk,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}
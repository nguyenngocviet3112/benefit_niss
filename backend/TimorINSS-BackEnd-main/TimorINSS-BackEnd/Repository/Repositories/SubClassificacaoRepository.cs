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
    public class SubClassificacaoRepository : ISubClassificacaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public SubClassificacaoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Subclassificacao> GetAll()
        {
            return _moduloContribuicoesContext.Subclassificacao.Where(u => u.IndActivo).ToList();
        }

        public Subclassificacao Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var subClassificacao = _moduloContribuicoesContext.Subclassificacao
                .SingleOrDefault(u => u.Id == id);

            return subClassificacao;
        }

        public SubClassificacaoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var subClassificacao = _moduloContribuicoesContext.Subclassificacao
                .SingleOrDefault(u => u.Id == id);

            SubClassificacaoDto subClassificacaoDto = Utils.MappClassToDto<Subclassificacao, SubClassificacaoDto>(subClassificacao);
            return subClassificacaoDto;
        }

        public void Add(Subclassificacao entity)
        {
            _moduloContribuicoesContext.Subclassificacao.Add(entity);
        }

        public void Update(Subclassificacao entity)
        {
            Subclassificacao entityToUpdate = _moduloContribuicoesContext.Subclassificacao
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Subclassificacao entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllSubClassificacao()
        {
            return _moduloContribuicoesContext.Subclassificacao
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = u.IndActivo,
                   parentId = u.ClassificacaoFk
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveSubClassificacao(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Subclassificacao> querySubclassificacaoAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            if (int.TryParse(filter.filterField, out int parent))
                querySubclassificacaoAux = _moduloContribuicoesContext.Subclassificacao
                    .Where(a => a.IndActivo && a.Nome.Contains(filter.filterBy) && a.ClassificacaoFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var querySubclassificacao = querySubclassificacaoAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Nome,
                   ParentId = u.ClassificacaoFk,
                   Parametros = new List<ParametrosAdicionais>()
               });

            var subClassificacoes = querySubclassificacao
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySubclassificacao.Count();

            response.ValuesCampo = subClassificacoes;
            response.CountValuesCampo = totalNumber;

            return response;
        }
    }
}
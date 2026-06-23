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
    public class ClassificacaoRepository : IClassificacaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public ClassificacaoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Classificacao> GetAll()
        {
            return _moduloContribuicoesContext.Classificacao.Where(u => u.IndActivo).ToList();
        }

        public Classificacao Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var classificacao = _moduloContribuicoesContext.Classificacao
                .SingleOrDefault(u => u.Id == id);

            return classificacao;
        }

        public ClassificacaoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var classificacao = _moduloContribuicoesContext.Classificacao
                .SingleOrDefault(u => u.Id == id);

            ClassificacaoDto classificacaoDto = Utils.MappClassToDto<Classificacao, ClassificacaoDto>(classificacao);
            return classificacaoDto;
        }

        public void Add(Classificacao entity)
        {
            _moduloContribuicoesContext.Classificacao.Add(entity);
        }

        public void Update(Classificacao entity)
        {
            Classificacao entityToUpdate = _moduloContribuicoesContext.Classificacao
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Classificacao entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllClassificacao()
        {
            return _moduloContribuicoesContext.Classificacao
                .Where(u => u.IndActivo)
               .Select(u => new SelectDescription
               {
                   id = u.Id,
                   nome = u.Nome,
                   indActivo = u.IndActivo
               })
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveClassificacao(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var querySector = _moduloContribuicoesContext.Classificacao
                .Where(s => s.IndActivo &&
                            // Filtrar por nome
                            s.Nome.Contains(filter.filterBy)
                )
                .Select(u => new ValorCamposEditaveis
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Parametros = new List<ParametrosAdicionais>()
                });

            var sectores = querySector
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySector.Count();

            response.ValuesCampo = sectores;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public List<SelectDescription> GetAllActiveClassificacao()
        {
            return _moduloContribuicoesContext.Classificacao
                .Where(u => u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.Id,
                    nome = u.Nome,
                    indActivo = u.IndActivo
                })
                .ToList();
        }
    }
}
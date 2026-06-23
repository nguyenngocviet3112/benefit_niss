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
    public class EscalaoRepository : IEscalaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public EscalaoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Escalao> GetAll()
        {
            return _moduloContribuicoesContext.Escalao.Where(e => e.IndActivo).ToList();
        }

        public Escalao Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var escalao = _moduloContribuicoesContext.Escalao
                .SingleOrDefault(e => e.IdEscalao == id);

            return escalao;
        }

        public EscalaoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var escalao = _moduloContribuicoesContext.Escalao
                .SingleOrDefault(u => u.IdEscalao == id);

            EscalaoDto escalaoDto = Utils.MappClassToDto<Escalao, EscalaoDto>(escalao);
            return escalaoDto;
        }

        public void Add(Escalao entity)
        {
            _moduloContribuicoesContext.Escalao.Add(entity);
        }

        public void Update(Escalao entity)
        {
            Escalao entityToUpdate = _moduloContribuicoesContext.Escalao
                .Single(d => d.IdEscalao == entity.IdEscalao);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Escalao entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllEscaloes()
        {
            List<SelectDescription> response = _moduloContribuicoesContext.Escalao
                .Select(e => new SelectDescription
                {
                    id = e.IdEscalao,
                    indActivo = e.IndActivo,
                    nome = e.DescNivelEscalao,
                    parentId = e.EscalaoRegimeFk
                }).ToList();
            return response;
        }

        public List<Escalao> GetAllEscaloesByParent(int idParent)
        {
            return _moduloContribuicoesContext.Escalao
                .Where(e => e.IndActivo && e.EscalaoRegimeFk == idParent)
                .ToList();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveEscaloes(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Escalao> queryEscalaoAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryEscalaoAux = _moduloContribuicoesContext.Escalao
                    .Where(a => a.IndActivo &&
                                a.DescNivelEscalao.Contains(filter.filterBy) &&
                                a.EscalaoRegimeFk == parent
            );
            else
                throw new Exception("No id was found for the entity parent");

            var queryEscalao = queryEscalaoAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdEscalao,
                   Nome = u.DescNivelEscalao,
                   ParentId = u.EscalaoRegimeFk,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                        new ParametrosAdicionais
                        {
                            Nome = "Valor",
                            Size = "19",
                            Type = "currency",
                            Valor = u.Valor.ToString()
                        }
                   }
               });

            var postos = queryEscalao
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryEscalao.Count();

            response.ValuesCampo = postos;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool IsEscalaoDeclared(Escalao escalao)
        {
            int count = _moduloContribuicoesContext.Declaracaoremuneracao
                 .Where(d => d.RegimeFk == escalao.EscalaoRegimeFk)
                 .Join(_moduloContribuicoesContext.Relentidadetrabalhador
                         .Where(r => r.EscalaoFk == escalao.IdEscalao),
                     d => d.DeclaracaoRelEntidadeTrabalhadorFk,
                     r => r.IdRel,
                     (d, r) => new
                     {
                         r.IdRel
                     }
                 )
                 .Count();

            return count == 0;
        }
    }
}
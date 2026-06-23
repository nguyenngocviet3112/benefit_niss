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
    public class CentroCustoRepository : ICentroCustoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public CentroCustoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Centrocusto> GetAll()
        {
            return _moduloContribuicoesContext.Centrocusto.ToList();
        }

        public Centrocusto Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var centrocusto = _moduloContribuicoesContext.Centrocusto
                .SingleOrDefault(u => u.Id == id);

            return centrocusto;
        }

        public CentroCustoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var centrocusto = _moduloContribuicoesContext.Centrocusto
                .SingleOrDefault(u => u.Id == id);

            CentroCustoDto centroCustoDto = Utils.MappClassToDto<Centrocusto, CentroCustoDto>(centrocusto);
            return centroCustoDto;
        }

        public void Add(Centrocusto entity)
        {
            _moduloContribuicoesContext.Centrocusto.Add(entity);
        }

        public void Update(Centrocusto entity)
        {
            Centrocusto entityToUpdate = _moduloContribuicoesContext.Centrocusto
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Centrocusto entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse GetAllActiveCentroCusto(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();
            IQueryable<Centrocusto> queryCentroCustoAux;

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            int parent = 0;
            if (int.TryParse(filter.filterField, out parent))
                queryCentroCustoAux = _moduloContribuicoesContext.Centrocusto
                    .Where(a => a.IndActivo && a.Descricao.Contains(filter.filterBy) && a.OrcamentoconfigFk == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var queryCentroCustoW = queryCentroCustoAux
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
                   Nome = u.Descricao,
                   ParentId = u.OrcamentoconfigFk,
                   Parametros = new List<ParametrosAdicionais>()
                   {
                        new ParametrosAdicionais
                        {
                            Nome = "DataInicio",
                            Size = "0",
                            Type = "date",
                            Valor = u.DataInicio.ToString("dd-MM-yyyy"),
                            DateValor = u.DataInicio
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "DataFim",
                            Size = "0",
                            Type = "date",
                            Valor = u.DataFim.HasValue ? u.DataFim.Value.ToString("dd-MM-yyyy") : "",
                            DateValor = u.DataFim
                        }
                   }
               });

            var centrosCusto = queryCentroCustoW
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryCentroCustoW.Count();

            response.ValuesCampo = centrosCusto;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public List<SelectDescription> GetAllActiveCentroCustoByOrcamentoRegisto(int orcamentoId)
        {
            ComponenteorcamentoRegisto orcamento = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Where(o => o.Id == orcamentoId).FirstOrDefault();

            DateTime maxDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            return _moduloContribuicoesContext.Centrocusto
                .Where(c => c.IndActivo &&
                            // Filtrar por Orçamento Config
                            c.OrcamentoconfigFk == orcamento.OrcamentoConfigFk &&
                            // Filtrar por data de orçamento
                            (c.DataInicio > orcamento.DataInicio ? c.DataInicio : orcamento.DataInicio) <=
                            ((c.DataFim.HasValue ? c.DataFim : maxDate) > orcamento.DataFim ? orcamento.DataFim : c.DataFim)
                )
                .Select(c => new SelectDescription
                {
                    id = c.Id,
                    nome = c.Descricao,
                    indActivo = c.IndActivo
                })
                .ToList();
        }

        public bool IsCentroCustoDeleteValid(Centrocusto centro)
        {
            int count = _moduloContribuicoesContext.Componenteorcamentovalor
                .Where(c => c.IndActivo && c.CentroCustoFk == centro.Id)
                .Count();

            return count == 0;
        }

        public List<Centrocusto> GetAllActive(DateTime? data = null)
        {
            return _moduloContribuicoesContext.Centrocusto
                                              .Where(e => e.IndActivo && 
                                                          // Filtrar por data de Orçamento Config
                                                          (data == null || (data >= e.OrcamentoconfigFkNavigation.DataInicio && data <= e.OrcamentoconfigFkNavigation.DataFim))
            ).ToList();
        }
    }
}
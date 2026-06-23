using Microsoft.AspNetCore.Http;
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
    public class TaxaJuroMensalRepository : ITaxaJuroMensalRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaxaJuroMensalRepository(TimorINSSModuloContribuicoesContext storeContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Taxajuromensal> GetAll()
        {
            _moduloContribuicoesContext.Taxajuromensal
               .ToList();

            return _moduloContribuicoesContext.Taxajuromensal;
        }

        public Taxajuromensal Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var taxaJuromensal = _moduloContribuicoesContext.Taxajuromensal
                .SingleOrDefault(u => u.IdTaxa == id);

            return taxaJuromensal;
        }

        public TaxajuromensalDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var taxaJuroMensal = _moduloContribuicoesContext.Taxajuromensal
                .SingleOrDefault(u => u.IdTaxa == id);

            TaxajuromensalDto taxaJuromensalDto = Utils.MappClassToDto<Taxajuromensal, TaxajuromensalDto>(taxaJuroMensal);
            return taxaJuromensalDto;
        }

        public void Add(Taxajuromensal entity)
        {
            _moduloContribuicoesContext.Taxajuromensal.Add(entity);
        }

        public void Update(Taxajuromensal entity)
        {
            Taxajuromensal entityToUpdate = _moduloContribuicoesContext.Taxajuromensal
                .Single(d => d.IdTaxa == entity.IdTaxa);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Taxajuromensal entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public Taxajuromensal GetTaxaJuroMensalByData(DateTime mesAno)
        {
            DateTime begin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (mesAno != null)
            {
                var beginFullDate = mesAno;
                begin = new DateTime(beginFullDate.Year, beginFullDate.Month, 1);
            }
            return _moduloContribuicoesContext.Taxajuromensal.
                SingleOrDefault(u => u.DataInicio <= begin && (u.DataFim >= begin || !u.DataFim.HasValue) && u.IndActivo);
        }

        public ValueCampoEditavelListagemResponse GetAllActiveTaxas(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            IQueryable<ValorCamposEditaveis> queryJuroFinal = _moduloContribuicoesContext.Taxajuromensal
            .Where(a => a.IndActivo)
            .Select(u => new ValorCamposEditaveis
            {
                Id = u.IdTaxa,
                Nome = u.DataFim.HasValue ? (u.DataInicio.ToString("MM-yyyy") + " => " + u.DataFim.Value.ToString("MM-yyyy")) : u.DataInicio.ToString("MM-yyyy"),
                Parametros = new List<ParametrosAdicionais>()
                {
                    new ParametrosAdicionais
                    {
                        Nome = "Percentagem",
                        Size = "5",
                        Type = "decimal",
                        Valor = u.Percentagem.ToString(),
                        Suffix = "%"
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "DataInicio",
                        Size = "0",
                        Type = "mesAno",
                        Valor = u.DataInicio.ToString("dd-MM-yyyy"),
                        DateValor = u.DataInicio
                    },
                    new ParametrosAdicionais
                    {
                        Nome = "DataFim",
                        Size = "0",
                        Type = "mesAno",
                        Valor = u.DataFim.HasValue ? u.DataFim.Value.ToString("dd-MM-yyyy") : "",
                        DateValor = u.DataFim ?? null
                    }
                }
            });

            var juros = queryJuroFinal
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryJuroFinal.Count();

            response.ValuesCampo = juros;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public bool IsTaxaDatesValid(Taxajuromensal taxa)
        {
            return !_moduloContribuicoesContext.Taxajuromensal
                .Any(t => t.IdTaxa != taxa.IdTaxa &&
                            t.IndActivo &&
                            (t.DataInicio <= (taxa.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) &&
                            (t.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= taxa.DataInicio)
                 );
        }
    }
}
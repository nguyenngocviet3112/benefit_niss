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
    public class RegimeRepository : IRegimeRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RegimeRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Regime> GetAll()
        {
            return _moduloContribuicoesContext.Regime.ToList();
        }

        public Regime Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var regime = _moduloContribuicoesContext.Regime
                .SingleOrDefault(u => u.IdRegime == id);

            return regime;
        }

        public RegimeDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var regime = _moduloContribuicoesContext.Regime
                .SingleOrDefault(u => u.IdRegime == id);

            RegimeDto regimeDto = Utils.MappClassToDto<Regime, RegimeDto>(regime);
            return regimeDto;
        }

        public void Add(Regime entity)
        {
            _moduloContribuicoesContext.Regime.Add(entity);
        }

        public void Update(Regime entity)
        {
            Regime entityToUpdate = _moduloContribuicoesContext.Regime
                .Single(d => d.IdRegime == entity.IdRegime);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Regime entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public List<SelectDescription> GetAllRegimes()
        {
            return _moduloContribuicoesContext.Regime
                .Select(c => new SelectDescription
                {
                    id = c.IdRegime,
                    nome = c.NomeRegime,
                    indActivo = c.IndActivo
                })
                .ToList();
        }

        public Regime GetRegimeByParentAndDate(int parent, DateTime data)
        {
            return _moduloContribuicoesContext.Regime
                .Where(r => r.RegimePai == parent &&
                            r.DataInicio <= data &&
                            (r.DataFim ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= data &&
                            r.IndActivo
                )
                .SingleOrDefault();
        }

        public List<Regime> GetAllRegimesByParent(int parent)
        {
            return _moduloContribuicoesContext.Regime
                .Where(r => r.RegimePai == parent)
                .ToList();
        }

        public List<Regime> GetAllActiveRegimesByParent(int parent)
        {
            return _moduloContribuicoesContext.Regime
                .Where(r => r.RegimePai == parent && r.IndActivo)
                .ToList();
        }

        public Dominio GetTipoRegimeFromRegimePai(long regimePai)
        {
            return _moduloContribuicoesContext.Regime
                .Include(r => r.TipoRegimeNavigation)
                .Where(r => r.RegimePai == regimePai)
                .OrderByDescending(r => r.DataInicio)
                .Select(r => r.TipoRegimeNavigation)
                .FirstOrDefault();
        }

        public ValueCampoEditavelListagemResponse GetAllActiveRegimes(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            IQueryable<Regime> queryRegime;


            if (int.TryParse(filter.filterField, out int parent))
                queryRegime = _moduloContribuicoesContext.Regime
                    .Where(r => r.IndActivo && r.NomeRegime.Contains(filter.filterBy) && r.RegimePai == parent);
            else
                throw new Exception("No id was found for the entity parent");

            var queryRefimeF = queryRegime.Where(a => a.IndActivo && a.NomeRegime.Contains(filter.filterBy))
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.IdRegime,
                   Nome = u.NomeRegime,
                   ParentId = u.RegimePai,
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
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "TaxaEntidade",
                            Size = "6",
                            Type = "decimal",
                            Valor = u.PercentEntidadeEmpreg.ToString(),
                            Suffix = "%"
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "TaxaTrabalhador",
                            Size = "6",
                            Type = "decimal",
                            Valor = u.PercentTrabalhador.ToString(),
                            Suffix = "%"
                        },
                        new ParametrosAdicionais
                        {
                            Nome = "DiaVencimento",
                            Size = "3",
                            Type = "number",
                            Valor = u.DataVencimento.ToString()
                        }
                   }
               });

            var regimes = queryRefimeF
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryRegime.Count();

            response.ValuesCampo = regimes;
            response.CountValuesCampo = totalNumber;

            return response;
        }
    }
}
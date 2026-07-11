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
    public class OrcamentoConfigRepository : IOrcamentoConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public OrcamentoConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Orcamentoconfig> GetAll()
        {
            return _moduloContribuicoesContext.Orcamentoconfig.ToList();
        }

        public Orcamentoconfig Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var orcamentoconfig = _moduloContribuicoesContext.Orcamentoconfig
                .SingleOrDefault(u => u.Id == id);

            return orcamentoconfig;
        }

        public OrcamentoConfigDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var orcamentoconfig = _moduloContribuicoesContext.Orcamentoconfig
                .SingleOrDefault(u => u.Id == id);

            OrcamentoConfigDto orcamentoconfigDto = Utils.MappClassToDto<Orcamentoconfig, OrcamentoConfigDto>(orcamentoconfig);
            return orcamentoconfigDto;
        }

        public void Add(Orcamentoconfig entity)
        {
            _moduloContribuicoesContext.Orcamentoconfig.Add(entity);
        }

        public void Update(Orcamentoconfig entity)
        {
            Orcamentoconfig entityToUpdate = _moduloContribuicoesContext.Orcamentoconfig
                .Single(d => d.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Orcamentoconfig entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ValueCampoEditavelListagemResponse getAllActiveOrcamentoConfig(SearchFilter filter)
        {
            ValueCampoEditavelListagemResponse response = new ValueCampoEditavelListagemResponse();

            int index = 0;
            if (filter.index.HasValue)
                index = filter.index.Value;

            int rows = 5;
            if (filter.rows.HasValue)
                rows = filter.rows.Value;

            var queryOrcamentoConfigW = _moduloContribuicoesContext.Orcamentoconfig.Where(a => a.IndActivo)
               .Select(u => new ValorCamposEditaveis
               {
                   Id = u.Id,
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

            var orcamentosConfig = queryOrcamentoConfigW
                .OrderBy("Id")
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryOrcamentoConfigW.Count();

            response.ValuesCampo = orcamentosConfig;
            response.CountValuesCampo = totalNumber;

            return response;
        }

        public List<SelectDescription> GetAllOrcamentoConfig()
        {
            return _moduloContribuicoesContext.Orcamentoconfig
                .Where(u => u.IndActivo)
                .Select(u => new SelectDescription
                {
                    id = u.Id,
                    nome = u.DataFim.HasValue ? (u.DataInicio.ToString("dd-MM-yyyy") + " => " + u.DataFim.Value.ToString("dd-MM-yyyy")) : u.DataInicio.ToString("dd-MM-yyyy"),
                    indActivo = u.IndActivo
                })
                .ToList();
        }

        public Error IsOrcamentoValid(Orcamentoconfig orcamento)
        {
            DateTime maxDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            int count = _moduloContribuicoesContext.Centrocusto
                .Where(c => c.OrcamentoconfigFk == orcamento.Id && c.IndActivo &&
                (c.DataInicio < orcamento.DataInicio || (c.DataFim.HasValue ? c.DataFim : maxDate) > (orcamento.DataFim.HasValue ? orcamento.DataFim : maxDate)))
                .Count();

            int count2 = _moduloContribuicoesContext.Orcamentoconfig
                .Where(o => o.Id != orcamento.Id && o.IndActivo &&
                (o.DataInicio <= (orcamento.DataFim.HasValue ? orcamento.DataFim.Value : System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                && (o.DataFim.HasValue ? o.DataFim.Value : System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= orcamento.DataInicio))
                .Count();

            int count3 = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Where(o => o.DataInicio >= orcamento.DataInicio && o.DataFim >= (orcamento.DataFim.HasValue ? orcamento.DataFim : maxDate) && o.IndActivo)
                .Count();

            if (count != 0 || count2 != 0)
                return new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataFimPeriodoVigencia).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataFimPeriodoVigencia.ToString()
                };

            if (count3 != 0)
                return new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataFimPeriodoVigenciaOrcamentos).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataFimPeriodoVigenciaOrcamentos.ToString()
                };

            return null;
        }

        public Error IsComponenteOrcamentoValid(DateTime startDate, DateTime? endDate)
        {
            DateTime maxDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            int count = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Where(o => o.DataInicio >= startDate && o.DataFim >= (endDate.HasValue ? endDate : maxDate) && o.IndActivo)
                .Count();

            if (count != 0)
                return new Error
                {
                    ErrorCode = ((int)ErrorsDataContract.ErroDataFimPeriodoVigenciaOrcamentos).ToString(),
                    ErrorMessage = ErrorsDataContract.ErroDataFimPeriodoVigenciaOrcamentos.ToString()
                };

            return null;
        }

        public Orcamentoconfig GetOrcamentoConfigByDates(DateTime start, DateTime end)
        {
            return _moduloContribuicoesContext.Orcamentoconfig
                 .Where(o => o.IndActivo && start >= o.DataInicio && (end <= o.DataFim || o.DataFim == null))
                 .FirstOrDefault();
        }


        public Orcamentoconfig GetOrcamentoConfigCurrentDate()
        {
            var today = DateTime.Today;
            return _moduloContribuicoesContext.Orcamentoconfig
                 .Where(o => o.IndActivo && today >= o.DataInicio && (today <= o.DataFim || o.DataFim == null))
                 .FirstOrDefault();
        }

        public bool IsOrcamentoDeleteValid(Orcamentoconfig orcamento)
        {
            DateTime maxDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            int count = _moduloContribuicoesContext.ComponenteorcamentoRegisto
                .Where(o => o.DataInicio >= orcamento.DataInicio &&
                            o.DataInicio <= (orcamento.DataFim.HasValue ? orcamento.DataFim : maxDate) &&
                            o.IndActivo
                )
                .Count();

            return count == 0;
        }

        public bool IsAnoTipoValid(Orcamentoconfig orcamento)
        {
            int count = _moduloContribuicoesContext.Orcamentoconfig
                .Where(o => o.Id != orcamento.Id && o.IndActivo
                    && o.Ano == orcamento.Ano
                    && o.Tipo == orcamento.Tipo)
                .Count();

            return count == 0;
        }

        public bool HasOrcamentoBatch(int id)
        {
            return _moduloContribuicoesContext.OrcamentoBatch
                .Any(b => b.OrcamentoConfigFk == id && b.IndActivo);
        }
    }
}
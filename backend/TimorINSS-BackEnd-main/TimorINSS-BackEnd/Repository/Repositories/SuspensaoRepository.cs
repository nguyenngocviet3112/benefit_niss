using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class SuspensaoRepository : ISuspensaoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SuspensaoRepository(TimorINSSModuloContribuicoesContext storeContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Suspensoes> GetAll()
        {
            _moduloContribuicoesContext.Suspensoes
               .Include(u => u.EntidadeSuspensaoFkNavigation)
               .Include(u => u.TrabalhadorSuspensaoFkNavigation)
               .Where(u => u.IndActivo)
               .ToList();

            return _moduloContribuicoesContext.Suspensoes;
        }

        public Suspensoes Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var suspensao = _moduloContribuicoesContext.Suspensoes
                .Include(u => u.EntidadeSuspensaoFkNavigation)
                .Include(u => u.TrabalhadorSuspensaoFkNavigation)
                .SingleOrDefault(u => u.IdSuspensao == id);

            return suspensao;
        }

        public SuspensoesDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var suspensao = _moduloContribuicoesContext.Suspensoes
                .SingleOrDefault(u => u.IdSuspensao == id);

            SuspensoesDto suspensaoDto = Utils.MappClassToDto<Suspensoes, SuspensoesDto>(suspensao);
            return suspensaoDto;
        }

        public void Add(Suspensoes entity)
        {
            _moduloContribuicoesContext.Suspensoes.Add(entity);
        }

        public void Update(Suspensoes entity)
        {
            Suspensoes entityToUpdate = _moduloContribuicoesContext.Suspensoes
                .Single(d => d.IdSuspensao == entity.IdSuspensao);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Suspensoes entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public SuspensaoListagemResponse GetSuspensaoByFilter(SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse result = new SuspensaoListagemResponse();
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            IQueryable<Suspensoes> querySuspensaoConditional = _moduloContribuicoesContext.Suspensoes;
            switch (request.filter.filterField)
            {
                case "ENTIDADEEMPREGADORA":
                    querySuspensaoConditional = querySuspensaoConditional.Where(u => u.EntidadeSuspensaoFk == request.IdEntidade && u.TrabalhadorSuspensaoFk == null && u.IndActivo);
                    break;

                case "TRABALHADOR":
                    querySuspensaoConditional = querySuspensaoConditional.Where(u => u.TrabalhadorSuspensaoFk == request.IdTrabalhador && u.IndActivo && u.EntidadeSuspensaoFk == request.IdEntidade);
                    break;

                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return result;
            }

            var querySuspensao = querySuspensaoConditional
                .Select(suspensao => new SuspensaoListagem
                {
                    idSuspensao = suspensao.IdSuspensao,
                    idEntidade = suspensao.EntidadeSuspensaoFk,
                    idTrabalhador = suspensao.TrabalhadorSuspensaoFk,
                    dataInicioSuspensao = suspensao.DataInicioSuspensao,
                    dataFimSuspensao = suspensao.DataFimSuspensao
                });

            var suspensao = querySuspensao
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySuspensao.Count();

            result.suspensao = suspensao;
            result.rows = totalNumber;
            return result;
        }

        public bool GetSuspensaoByData(SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse result = new SuspensaoListagemResponse();
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            IQueryable<Suspensoes> querySuspensaoConditional = _moduloContribuicoesContext.Suspensoes;
            switch (request.filter.filterField)
            {
                case "ENTIDADEEMPREGADORA":
                    querySuspensaoConditional = querySuspensaoConditional.Where(u => u.EntidadeSuspensaoFk == request.IdEntidade && u.TrabalhadorSuspensaoFk == null && u.IndActivo && (DateTime.Now <= u.DataFimSuspensao || (u.DataInicioSuspensao != null && u.DataFimSuspensao == null)));
                    break;

                case "TRABALHADOR":
                    querySuspensaoConditional = querySuspensaoConditional.Where(u => u.TrabalhadorSuspensaoFk == request.IdTrabalhador && u.EntidadeSuspensaoFk == request.IdEntidade && u.IndActivo && (DateTime.Now <= u.DataFimSuspensao || (u.DataInicioSuspensao != null && u.DataFimSuspensao == null)));
                    break;

                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return false;
            }

            var querySuspensao = querySuspensaoConditional
                .Select(suspensao => new SuspensaoListagem
                {
                    idSuspensao = suspensao.IdSuspensao,
                    idEntidade = suspensao.EntidadeSuspensaoFk,
                    idTrabalhador = suspensao.TrabalhadorSuspensaoFk,
                    dataInicioSuspensao = suspensao.DataInicioSuspensao,
                    dataFimSuspensao = suspensao.DataFimSuspensao
                });

            var suspensao = querySuspensao
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = querySuspensao.Count();

            if (totalNumber > 0)
            {
                return true;
            }
            return false;
        }

        public bool ExistSuspensao(DateTime begin, DateTime end, int? idEntidade, int? idTrabalhador)
        {
            List<Suspensoes> results = _moduloContribuicoesContext.Suspensoes
                    .Where(u => u.DataInicioSuspensao < end &&
                            begin < (u.DataFimSuspensao ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            && u.EntidadeSuspensaoFk == idEntidade && u.TrabalhadorSuspensaoFk == idTrabalhador && u.IndActivo)
                    .ToList();
            return results.Count() > 0;
        }

        public List<int> GetAllRelEntidadeTrabalhadorSuspensosByDate(int idEntidade, DateTime begin, DateTime end)
        {
            List<int> result = new List<int>();

            result = _moduloContribuicoesContext.Suspensoes
                    .Where(s => s.DataInicioSuspensao <= begin &&
                                end >= (s.DataFimSuspensao ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) &&
                                s.EntidadeSuspensaoFk == idEntidade &&
                                s.IndActivo &&
                                s.TrabalhadorSuspensaoFk.HasValue
                    )
                    .Join(
                        _moduloContribuicoesContext.Relentidadetrabalhador,
                        suspensao => suspensao.TrabalhadorSuspensaoFk,
                        rel => rel.TrabalhadorFk,
                        (suspensao, rel) => new { suspensao, rel }
                    )
                    .Where(r => r.suspensao.DataInicioSuspensao >= r.rel.DtIniVincTrabalhador &&
                                r.rel.EntidadeFk == r.suspensao.EntidadeSuspensaoFk &&
                                (r.rel.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) >= (r.suspensao.DataFimSuspensao ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                    )
                    .Select(r => r.rel.IdRel)
                    .ToList();

            return result;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class RelEntidadeTrabalhadorRepository : IRelEntidadeTrabalhadorRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public RelEntidadeTrabalhadorRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Relentidadetrabalhador> GetAll()
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                .ToList();
        }

        public Relentidadetrabalhador Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var relentidadetrabalhador = _moduloContribuicoesContext.Relentidadetrabalhador
                .SingleOrDefault(u => u.IdRel == id);

            return relentidadetrabalhador;
        }

        public RelentidadetrabalhadorDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;
            var relentidadetrabalhador = Get(id);

            RelentidadetrabalhadorDto relentidadetrabalhadorDto = Utils.MappClassToDto<Relentidadetrabalhador, RelentidadetrabalhadorDto>(relentidadetrabalhador);
            return relentidadetrabalhadorDto;
        }

        public void Add(Relentidadetrabalhador entity)
        {
            _moduloContribuicoesContext.Relentidadetrabalhador.Add(entity);
        }

        public void Update(Relentidadetrabalhador entity)
        {
            Relentidadetrabalhador entityToUpdate = _moduloContribuicoesContext.Relentidadetrabalhador
                .Single(d => d.IdRel == entity.IdRel);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Relentidadetrabalhador entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public bool IsTrabalhadorVinculado(DateTime begin, DateTime end, int trabalhador, int empresa, int idRel = 0)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                                              .Any(u => u.DtIniVincTrabalhador < end &&
                                                        begin < (u.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value) &&
                                                        u.TrabalhadorFk == trabalhador &&
                                                        u.IdRel != idRel &&
                                                        u.EntidadeFk == empresa
                                               );
        }

        public bool IsTrabalhadorAssociadoEntidadeEmpregadora(int idTrabalhador, int idEntidadeEmpregadora)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                       .Any(u => u.TrabalhadorFk == idTrabalhador &&
                                 u.EntidadeFk == idEntidadeEmpregadora
                       );
        }

        public bool NumFuncPublicoExists(string numFuncPublico, int trabalhadorId = 0)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                        .Any(t => t.TrabalhadorFk != trabalhadorId &&
                                  string.Compare(t.NumFuncPublico, numFuncPublico) == 0
                         );
        }

        public List<int> GetAllRelTrabalhadorByEntidadeAndMonth(int entidadeId, DateTime date, GetDeclaracaoByEntidadeAndFilterRequest request)
        {
            DateTime begin = date;
            DateTime end = date.AddMonths(1).AddDays(-1);

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 10;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;



            return _moduloContribuicoesContext.Relentidadetrabalhador
                    .Where(r => r.EntidadeFk == entidadeId && r.DtIniVincTrabalhador <= end &&
                        begin < (r.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value))
                    .OrderBy(r => r.IdRel)
                    .Select(r => r.IdRel)
                    .Skip(index * rows)
                    .Take(rows)
                    .ToList();
        }

        public List<int> GetAllRelTrabalhadorByEntidadeAndMonthAll(int entidadeId, DateTime date)
        {
            DateTime begin = date;
            DateTime end = date.AddMonths(1).AddDays(-1);


            return _moduloContribuicoesContext.Relentidadetrabalhador
                    .Where(r => r.EntidadeFk == entidadeId && r.DtIniVincTrabalhador <= end &&
                        begin < (r.DtIniFimTrabalhador ?? System.Data.SqlTypes.SqlDateTime.MaxValue.Value))
                    .Select(r => r.IdRel)
                    .ToList();
        }

        public int GetTrabalhadorIdByRelId(int relId)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                .Select(r => r.TrabalhadorFk).FirstOrDefault();
        }

        public DateTime? GetFirstContratoDateByEntidade(int entidadeId)
        {
            var query = _moduloContribuicoesContext.Relentidadetrabalhador
                .Where(r => r.EntidadeFk == entidadeId);

            if (query.Any())
                return query.Min(r => r.DtIniVincTrabalhador);
            else
                return null;
        }

        public int GetEntidadeINSSIdByTrabalhadorInterno(int trabalhadorId)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                .Where(r => r.TrabalhadorFk == trabalhadorId).FirstOrDefault().EntidadeFk;
        }

        public bool EntidadeTemConflitoComTipoRegime(int entidadeId, int regimeId)
        {
            Regime regime = _moduloContribuicoesContext.Regime
            .Where(r => r.RegimePai == regimeId)
            .FirstOrDefault();

            if (regime != null)
            {
                int tipoRegimeId = regime.TipoRegime;

                List<int> regimesIDs = _moduloContribuicoesContext.Relentidadetrabalhador
                    .Where(r => r.EntidadeFk == entidadeId)
                    .Select(r => r.RegimeFk).ToList();

                return _moduloContribuicoesContext.Regime
                    .Any(r => regimesIDs.Contains(r.RegimePai) &&
                                r.TipoRegime != tipoRegimeId
                    );
            }
            else
                return true;
        }

        public List<Relentidadetrabalhador> GetRelEntidadeTrabalhadorINSS(string Niss)
        {
            return _moduloContribuicoesContext.Relentidadetrabalhador
                .Include(u => u.EntidadeFkNavigation)
                .Include(u => u.TrabalhadorFkNavigation)
                .Where(r => r.EntidadeFkNavigation.Niss == Niss)
                .ToList();
        }
    }
}
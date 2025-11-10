using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class MoradaRepository : IMoradaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MoradaRepository(TimorINSSModuloContribuicoesContext storeContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Morada> GetAll()
        {
            _moduloContribuicoesContext.Morada
               .Include(u => u.MoradaAldeiaFkNavigation)
               .Include(u => u.MoradaPaisFkNavigation)
               .ToList();

            return _moduloContribuicoesContext.Morada;
        }

        public Morada Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var morada = _moduloContribuicoesContext.Morada
                .Include(u => u.MoradaAldeiaFkNavigation)
                .Include(u => u.MoradaPaisFkNavigation)
                .SingleOrDefault(u => u.IdMorada == id);

            return morada;
        }

        public MoradaDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var morada = _moduloContribuicoesContext.Morada
                .SingleOrDefault(u => u.IdMorada == id);

            MoradaDto moradaDto = Utils.MappClassToDto<Morada, MoradaDto>(morada);
            return moradaDto;
        }

        public void Add(Morada entity)
        {
            _moduloContribuicoesContext.Morada.Add(entity);
        }

        public void Update(Morada entity)
        {
            Morada entityToUpdate = _moduloContribuicoesContext.Morada
                .Single(d => d.IdMorada == entity.IdMorada);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Morada entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public MoradaListagemResponse GetMoradasByFilter(MoradaListagemRequest request)
        {
            MoradaListagemResponse result = new MoradaListagemResponse();

            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            request.filter.orderDirection = OrderDirectionEnum.descending;
            request.filter.orderBy = "moradaPrincipal";
            const int SECRET_A = 654321;
            const int SECRET_B = 123456789;
            int decodedId = (request.Id - SECRET_B) / SECRET_A;

            IQueryable<Morada> queryMoradasConditional = _moduloContribuicoesContext.Morada;
            switch (request.filter.filterField)
            {
                case "ENTIDADEEMPREGADORA":
                    queryMoradasConditional = queryMoradasConditional.Where(u => u.EntidadeMoradaFk == decodedId);
                    break;

                case "TRABALHADOR":
                    queryMoradasConditional = queryMoradasConditional.Where(u => u.TrabalhadorMoradaFk == decodedId);
                    break;

                default:
                    result.Errors.Add(new Error
                    {
                        ErrorCode = ((int)ErrorsDataContract.InvalidFilter).ToString(),
                        ErrorMessage = ErrorsDataContract.InvalidFilter.ToString()
                    });
                    return result;
            }

            var queryMoradas = queryMoradasConditional.Include(u => u.MoradaAldeiaFkNavigation)
                    .ThenInclude(suco => suco.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation)
                    .ThenInclude(posto => posto.PostoAdminMunicipioFkNavigation)
                .Include(u => u.MoradaPaisFkNavigation)
                .Select(morada => new MoradaListagem
                {
                    idMorada = morada.IdMorada,
                    rua = morada.Rua,
                    ruaNumPorta = morada.NumPorta == null ? morada.Rua : morada.Rua + ", " + morada.NumPorta,
                    aldeia = morada.MoradaAldeiaFkNavigation.Nome,
                    idAldeia = morada.MoradaAldeiaFkNavigation.IdAldeia,
                    municipio = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.Nome,
                    idMunicipio = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.PostoAdminMunicipioFkNavigation.IdMunicipio,
                    postoAdministrativo = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.Nome,
                    idPostoAdministrativo = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation.IdPostoAdmin,
                    suco = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.Nome,
                    idSuco = morada.MoradaAldeiaFkNavigation.AldeiaSucoFkNavigation.IdSuco,
                    pais = morada.MoradaPaisFkNavigation.Nome,
                    idPais = morada.MoradaPaisFkNavigation.IdPais,
                    moradaPrincipal = morada.MoradaPrincipal,
                    numPorta = morada.NumPorta
                });

            var morada = queryMoradas
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = queryMoradas.Count();

            result.morada = morada;
            result.rows = totalNumber;

            return result;
        }

        public void UpdateMoradaPrincipal(Morada morada)
        {
            var entity = _moduloContribuicoesContext.Morada
                .FirstOrDefault(item => item.MoradaPrincipal && item.EntidadeMoradaFk == morada.EntidadeMoradaFk && item.TrabalhadorMoradaFk == morada.TrabalhadorMoradaFk);

            if (entity != null)
            {
                entity.MoradaPrincipal = false;

                // Save changes in database
                _moduloContribuicoesContext.SaveChanges();
            }
        }

        public Morada GetMoradasIguais(Morada morada)
        {
            int? trabalhadorFk = null;
            int? entidadeFk = null;
            if (morada.TrabalhadorMoradaFk > 0)
            {
                trabalhadorFk = morada.TrabalhadorMoradaFk;
            }
            else if (morada.EntidadeMoradaFk > 0)
            {
                entidadeFk = morada.EntidadeMoradaFk;
            }
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var queryMorada = _moduloContribuicoesContext.Morada
                .Include(u => u.MoradaAldeiaFkNavigation)
                .Include(u => u.MoradaPaisFkNavigation)
                .SingleOrDefault(u => u.Rua == morada.Rua &&
                                      u.NumPorta == morada.NumPorta &&
                                      u.MoradaPaisFk == morada.MoradaPaisFk &&
                                      u.MoradaAldeiaFk == morada.MoradaAldeiaFk &&
                                      u.EntidadeMoradaFk == entidadeFk &&
                                      u.TrabalhadorMoradaFk == trabalhadorFk
                );

            return queryMorada;
        }

        public Morada GetMoradasPrincipalByEntidadeFk(int entidadeFk)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.Morada
                .Include(u => u.MoradaAldeiaFkNavigation)
                .ThenInclude(suco => suco.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation)
                .ThenInclude(posto => posto.PostoAdminMunicipioFkNavigation)
                .Include(u => u.MoradaPaisFkNavigation)
                .SingleOrDefault(u => u.MoradaPrincipal && u.EntidadeMoradaFk == entidadeFk);
        }

        public Morada GetMoradasPrincipalByTrabalhadorFk(int trabalhadorFk)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.Morada
                .Include(u => u.MoradaAldeiaFkNavigation)
                .ThenInclude(suco => suco.AldeiaSucoFkNavigation.SucoPostoAdminFkNavigation)
                .ThenInclude(posto => posto.PostoAdminMunicipioFkNavigation)
                .Include(u => u.MoradaPaisFkNavigation)
                .SingleOrDefault(u => u.MoradaPrincipal && u.TrabalhadorMoradaFk == trabalhadorFk);
        }
    }
}
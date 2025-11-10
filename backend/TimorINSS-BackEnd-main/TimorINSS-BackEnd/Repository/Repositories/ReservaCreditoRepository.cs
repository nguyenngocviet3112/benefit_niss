using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class ReservaCreditoRepository : IReservaCreditoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservaCreditoRepository(TimorINSSModuloContribuicoesContext storeContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduloContribuicoesContext = storeContext;
        }

        public IEnumerable<Reservacredito> GetAll()
        {
            _moduloContribuicoesContext.Reservacredito
               .Include(u => u.ReservaGuiaPagamentoFkNavigation)
               .Include(u => u.ReservaEntidadeFkNavigation)
               .ToList();

            return _moduloContribuicoesContext.Reservacredito;
        }

        public Reservacredito Get(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var reservaCredito = _moduloContribuicoesContext.Reservacredito
                .Include(u => u.ReservaGuiaPagamentoFkNavigation)
                .Include(u => u.ReservaEntidadeFkNavigation)
                .SingleOrDefault(u => u.IdReserva == id);

            return reservaCredito;
        }

        public Reservacredito GetActiveByEntidadeId(long entidadeId)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            var reservaCredito = _moduloContribuicoesContext.Reservacredito
                .Include(u => u.ReservaGuiaPagamentoFkNavigation)
                .Include(u => u.ReservaEntidadeFkNavigation)
                .SingleOrDefault(u => u.ReservaEntidadeFk == entidadeId && u.IndActivo == true);

            return reservaCredito;
        }

        public ReservaCreditoDto GetDto(long id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = true;

            var reservaCredito = _moduloContribuicoesContext.Reservacredito
                .SingleOrDefault(u => u.IdReserva == id);

            ReservaCreditoDto reservaCreditoDto = Utils.MappClassToDto<Reservacredito, ReservaCreditoDto>(reservaCredito);
            return reservaCreditoDto;
        }

        public void Add(Reservacredito entity)
        {
            _moduloContribuicoesContext.Reservacredito.Add(entity);
        }

        public void Update(Reservacredito entity)
        {
            Reservacredito entityToUpdate = _moduloContribuicoesContext.Reservacredito
                .Single(d => d.IdReserva == entity.IdReserva);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public void Delete(Reservacredito entity)
        {
            _moduloContribuicoesContext.Remove(entity);
        }

        public ReservaCreditoListagemResponse GetReservaCreditoByIdEntidade(ReservaCreditoListagemRequest request)
        {
            int index = 0;
            if (request.filter.index.HasValue)
                index = request.filter.index.Value;

            int rows = 5;
            if (request.filter.rows.HasValue)
                rows = request.filter.rows.Value;

            const int SECRET_A = 654321;
            const int SECRET_B = 123456789;
            int decodedId = (request.IdEntidade - SECRET_B) / SECRET_A;

            var queryReservaCredito = _moduloContribuicoesContext.Reservacredito
                .Where(u => u.ReservaEntidadeFk == decodedId)
                .Select(reservaCredito => new ReservaCreditoListagem
                {
                    IdEntidade = reservaCredito.ReservaEntidadeFk,
                    Valor = reservaCredito.Valor,
                    IndAtivo = reservaCredito.IndActivo
                });

            if (request.filter.filterBy == "indActivo")
            {
                queryReservaCredito = queryReservaCredito.Where(u => u.IndAtivo == true);
            }

            var reservaCredito = queryReservaCredito
                .OrderBy(request.filter.orderBy, request.filter.orderDirection)
                .Skip(index * rows)
                .Take(rows)
                .ToList();

            var totalNumber = reservaCredito.Count();

            ReservaCreditoListagemResponse response = new ReservaCreditoListagemResponse
            {
                ReservaCredito = reservaCredito,
                rows = totalNumber
            };
            return response;
        }

        public List<Reservacredito> GetByIds(List<int> reservaIds)
        {
            var reservasCredito = _moduloContribuicoesContext.Reservacredito
               .Where(x => reservaIds.Contains(x.IdReserva))
               .ToList();

            return reservasCredito;
        }
    }
}
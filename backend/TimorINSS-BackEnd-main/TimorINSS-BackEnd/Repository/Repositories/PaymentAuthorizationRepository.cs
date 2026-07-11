using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class PaymentAuthorizationRepository : IPaymentAuthorizationRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public PaymentAuthorizationRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<PaymentAuthorization> BaseQuery()
        {
            return _moduloContribuicoesContext.PaymentAuthorization
                .Include(p => p.ObligationFkNavigation)
                .Include(p => p.CodigoContaDebitoFkNavigation)
                .Include(p => p.CodigoContaCreditoFkNavigation)
                .Include(p => p.PaymentExecution).ThenInclude(e => e.ContaBancariaFkNavigation);
        }

        public List<PaymentAuthorization> GetByAno(int ano)
        {
            return BaseQuery()
                .Where(p => p.IndActivo && p.Ano == ano)
                .OrderBy(p => p.Mes).ThenBy(p => p.Numero)
                .ToList();
        }

        public PaymentAuthorization Get(int id)
        {
            return BaseQuery().SingleOrDefault(p => p.Id == id);
        }

        public void Add(PaymentAuthorization entity)
        {
            _moduloContribuicoesContext.PaymentAuthorization.Add(entity);
        }

        public void Update(PaymentAuthorization entity)
        {
            PaymentAuthorization entityToUpdate = _moduloContribuicoesContext.PaymentAuthorization
                .Single(p => p.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public int GetNextNumero(int mes, int ano)
        {
            int? max = _moduloContribuicoesContext.PaymentAuthorization
                .Where(p => p.IndActivo && p.Mes == mes && p.Ano == ano)
                .Select(p => (int?)p.Numero)
                .Max();

            return (max ?? 0) + 1;
        }

        public bool HasAuthorizationForObligation(int obligationFk)
        {
            return _moduloContribuicoesContext.PaymentAuthorization
                .Any(p => p.IndActivo && p.ObligationFk == obligationFk);
        }
    }
}

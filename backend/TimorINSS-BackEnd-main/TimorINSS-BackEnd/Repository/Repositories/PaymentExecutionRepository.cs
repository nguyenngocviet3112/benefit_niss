using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class PaymentExecutionRepository : IPaymentExecutionRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public PaymentExecutionRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<PaymentExecution> BaseQuery()
        {
            return _moduloContribuicoesContext.PaymentExecution
                .Include(e => e.ContaBancariaFkNavigation)
                .Include(e => e.PaymentAuthorizationFkNavigation).ThenInclude(a => a.ObligationFkNavigation);
        }

        public void Add(PaymentExecution entity)
        {
            _moduloContribuicoesContext.PaymentExecution.Add(entity);
        }

        public bool HasExecutionForAuthorization(int paymentAuthorizationFk)
        {
            return _moduloContribuicoesContext.PaymentExecution
                .Any(e => e.IndActivo && e.PaymentAuthorizationFk == paymentAuthorizationFk);
        }

        public List<PaymentExecution> GetAll()
        {
            return BaseQuery()
                .Where(e => e.IndActivo)
                .OrderByDescending(e => e.DataPagamento)
                .ToList();
        }

        public PaymentExecution Get(int id)
        {
            return BaseQuery().SingleOrDefault(e => e.Id == id);
        }

        public PaymentExecution GetByAuthorization(int paymentAuthorizationFk)
        {
            return BaseQuery().SingleOrDefault(e => e.IndActivo && e.PaymentAuthorizationFk == paymentAuthorizationFk);
        }
    }
}

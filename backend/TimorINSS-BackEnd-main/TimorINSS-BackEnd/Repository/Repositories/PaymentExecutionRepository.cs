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

        public void Add(PaymentExecution entity)
        {
            _moduloContribuicoesContext.PaymentExecution.Add(entity);
        }

        public bool HasExecutionForAuthorization(int paymentAuthorizationFk)
        {
            return _moduloContribuicoesContext.PaymentExecution
                .Any(e => e.IndActivo && e.PaymentAuthorizationFk == paymentAuthorizationFk);
        }
    }
}

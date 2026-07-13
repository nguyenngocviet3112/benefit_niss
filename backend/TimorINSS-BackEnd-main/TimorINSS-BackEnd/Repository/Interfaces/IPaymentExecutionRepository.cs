using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPaymentExecutionRepository
    {
        void Add(PaymentExecution entity);
        bool HasExecutionForAuthorization(int paymentAuthorizationFk);
        List<PaymentExecution> GetAll();
        PaymentExecution Get(int id);
        PaymentExecution GetByAuthorization(int paymentAuthorizationFk);
    }
}

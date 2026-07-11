using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPaymentExecutionRepository
    {
        void Add(PaymentExecution entity);
        bool HasExecutionForAuthorization(int paymentAuthorizationFk);
    }
}

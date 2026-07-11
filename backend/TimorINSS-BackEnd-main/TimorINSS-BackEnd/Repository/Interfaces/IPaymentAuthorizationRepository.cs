using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPaymentAuthorizationRepository
    {
        List<PaymentAuthorization> GetByAno(int ano);
        PaymentAuthorization Get(int id);
        void Add(PaymentAuthorization entity);
        void Update(PaymentAuthorization entity);
        int GetNextNumero(int mes, int ano);
        bool HasAuthorizationForObligation(int obligationFk);
    }
}

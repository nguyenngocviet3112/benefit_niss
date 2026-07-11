using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IExpenditureAuthorizationRepository
    {
        List<ExpenditureAuthorization> GetByAno(int ano);
        ExpenditureAuthorization Get(int id);
        void Add(ExpenditureAuthorization entity);
        void Update(ExpenditureAuthorization entity);
        int GetNextNumero(int mes, int ano);
        bool HasAuthorizationForRubrica(int orcamentoLinhaFk);

        void AddPlurianualidade(ExpenditureAuthorizationPlurianualidade entity);
        void UpdatePlurianualidade(ExpenditureAuthorizationPlurianualidade entity);
        ExpenditureAuthorizationPlurianualidade GetPlurianualidade(int id);
    }
}

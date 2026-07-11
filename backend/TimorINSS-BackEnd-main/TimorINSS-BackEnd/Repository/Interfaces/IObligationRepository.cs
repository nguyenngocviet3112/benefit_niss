using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IObligationRepository
    {
        List<Obligation> GetByAno(int ano);
        Obligation Get(int id);
        void Add(Obligation entity);
        void Update(Obligation entity);
        int GetNextNumero(int mes, int ano);

        void AddItem(ObligationItem entity);
        void UpdateItem(ObligationItem entity);
        ObligationItem GetItem(int id);
        decimal GetTotalCommittedForCompromisso(int compromissoDespesaFk);

        void AddBeneficiary(ObligationBeneficiary entity);
        void UpdateBeneficiary(ObligationBeneficiary entity);
        ObligationBeneficiary GetBeneficiary(int id);
    }
}

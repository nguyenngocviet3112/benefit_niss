using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IBudgetPeriodRepository
    {
        IEnumerable<BudgetPeriod> GetAll();

        BudgetPeriod Get(long id);

        void Add(BudgetPeriod entity);

        void Update(BudgetPeriod entity);

        bool IsAnoTipoValid(BudgetPeriod period);

        bool HasOrcamentoBatch(int id);

        // Covers the 3 other new-mode tables that anchor to BudgetPeriod besides
        // OrcamentoBatch (checked separately via HasOrcamentoBatch, same as the
        // pre-existing OrcamentoConfig behaviour this was split from).
        bool HasDependents(int id);
    }
}

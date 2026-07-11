using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoLinhaRepository
    {
        List<OrcamentoLinha> GetByBatch(int batchId);
        List<OrcamentoLinha> GetApprovedByOrcamentoConfig(int orcamentoConfigFk);
        OrcamentoLinha Get(int id);
        void Add(OrcamentoLinha entity);
        void Update(OrcamentoLinha entity);
        bool IsComboValid(OrcamentoLinha entity);
    }
}

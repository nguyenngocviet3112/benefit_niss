using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IEconomicClassificationRepository
    {
        List<EconomicClassification> GetTreeByOrcamentoConfig(int orcamentoConfigFk);
        EconomicClassification Get(int id);
        void Add(EconomicClassification entity);
        void Update(EconomicClassification entity);
        bool IsCodeValid(EconomicClassification entity);
        bool HasActiveChildren(int id);
    }
}

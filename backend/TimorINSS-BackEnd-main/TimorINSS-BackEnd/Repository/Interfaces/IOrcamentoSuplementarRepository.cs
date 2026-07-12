using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoSuplementarRepository
    {
        OrcamentoSuplementar GetActiveDraftBatch(int orcamentoConfigFk);
        OrcamentoSuplementar Get(int id);
        void Add(OrcamentoSuplementar entity);
        void Update(OrcamentoSuplementar entity);

        List<OrcamentoSuplementarLinha> GetLinhasByBatch(int batchId);
        OrcamentoSuplementarLinha GetLinha(int id);
        void AddLinha(OrcamentoSuplementarLinha entity);
        void UpdateLinha(OrcamentoSuplementarLinha entity);
    }
}

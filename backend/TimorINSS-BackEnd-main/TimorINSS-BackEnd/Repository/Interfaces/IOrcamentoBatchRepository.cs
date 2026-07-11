using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoBatchRepository
    {
        // Batch mais recente que ainda não está APPROVED (para continuar a editar).
        OrcamentoBatch GetActiveDraftBatch(int orcamentoConfigFk);
        OrcamentoBatch Get(int id);
        void Add(OrcamentoBatch entity);
        void Update(OrcamentoBatch entity);
    }
}

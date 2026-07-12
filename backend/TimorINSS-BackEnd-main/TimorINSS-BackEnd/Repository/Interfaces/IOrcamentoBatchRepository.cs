using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoBatchRepository
    {
        // Batch mais recente que ainda não está APPROVED (para continuar a editar).
        OrcamentoBatch GetActiveDraftBatch(int orcamentoConfigFk);
        // Batch mais recente independentemente do Estado (para apenas visualizar —
        // não deve criar um novo DRAFT vazio só porque o último já está APPROVED).
        OrcamentoBatch GetLatestBatch(int orcamentoConfigFk);
        OrcamentoBatch Get(int id);
        void Add(OrcamentoBatch entity);
        void Update(OrcamentoBatch entity);
    }
}

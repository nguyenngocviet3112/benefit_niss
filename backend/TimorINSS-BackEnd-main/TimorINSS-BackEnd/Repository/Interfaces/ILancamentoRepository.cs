using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ILancamentoRepository
    {
        void Add(Lancamento entity);
        bool ExistsForOrigem(string origemTipo, int origemId);
        Lancamento GetByOrigem(string origemTipo, int origemId);
        List<Lancamento> GetByFilter(int? ano, int? mes, string origemTipo);
        void DeactivateForOrigem(string origemTipo, int origemId);
    }
}

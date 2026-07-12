using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICabimentoRepository
    {
        List<Cabimento> GetByAno(int ano);
        Cabimento Get(int id);
        void Add(Cabimento entity);
        void Update(Cabimento entity);
        int GetNextNumero(int mes, int ano);
        bool HasCabimentoForAd(int expenditureAuthorizationFk);
        Dictionary<int, decimal> GetComprometidoByCabimentoIds(List<int> cabimentoIds);
    }
}

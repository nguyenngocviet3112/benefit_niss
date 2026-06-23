using System.Collections.Generic;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IUtilizadorTokenRepository : IDataRepository<Utilizadortoken, UtilizadortokenDto>
    {
        public List<Utilizadortoken> GetByUserId(long userId);
        public List<Utilizadortoken> GetUniqueTokensByUserId(long userId);
        public List<Utilizadortoken> GetUniqueTokensByEntidadeId(int entidadeId);
    }
}
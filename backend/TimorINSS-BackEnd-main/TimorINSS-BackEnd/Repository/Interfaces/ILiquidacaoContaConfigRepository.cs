using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ILiquidacaoContaConfigRepository
    {
        List<LiquidacaoContaConfig> GetAll();
        LiquidacaoContaConfig GetByCategoria(string categoria);
        void Update(LiquidacaoContaConfig entity);
    }
}

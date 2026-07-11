using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IReceitaPacRepository
    {
        List<ReceitaPac> GetByAno(int ano);
        ReceitaPac Get(int id);
        void Add(ReceitaPac entity);
        void Update(ReceitaPac entity);
        int GetNextNumero(int mes, int ano);
    }
}

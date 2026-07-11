using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ICompromissoDespesaRepository
    {
        List<CompromissoDespesa> GetByAno(int ano);
        CompromissoDespesa Get(int id);
        void Add(CompromissoDespesa entity);
        void Update(CompromissoDespesa entity);
        int GetNextNumero(int mes, int ano);

        void AddPlurianualidade(CompromissoDespesaPlurianualidade entity);
        void UpdatePlurianualidade(CompromissoDespesaPlurianualidade entity);
        CompromissoDespesaPlurianualidade GetPlurianualidade(int id);
    }
}

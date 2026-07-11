using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IProgramActivityRepository
    {
        List<ProgramActivity> GetTreeByOrcamentoConfig(int orcamentoConfigFk);
        ProgramActivity Get(int id);
        void Add(ProgramActivity entity);
        void Update(ProgramActivity entity);
        bool IsCodeValid(ProgramActivity entity);
        bool HasActiveChildren(int id);
    }
}

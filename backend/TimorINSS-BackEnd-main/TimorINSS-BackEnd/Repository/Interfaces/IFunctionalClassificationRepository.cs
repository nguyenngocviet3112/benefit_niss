using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IFunctionalClassificationRepository
    {
        List<FunctionalClassification> GetAllActive();
        FunctionalClassification Get(int id);
        void Add(FunctionalClassification entity);
        void Update(FunctionalClassification entity);
        bool IsCodeValid(FunctionalClassification entity);
        bool HasActiveChildren(int id);
    }
}

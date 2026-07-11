using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface ILanguageConfigRepository
    {
        List<LanguageConfig> GetAll();
        List<LanguageConfig> GetAllActive();
        LanguageConfig Get(int id);
        void Update(LanguageConfig entity);
    }
}

using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IIntegrationConfigRepository
    {
        IntegrationConfig Get();
        void Update(IntegrationConfig entity);
    }
}

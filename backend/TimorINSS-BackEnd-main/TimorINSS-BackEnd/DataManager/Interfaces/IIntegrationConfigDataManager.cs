using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IIntegrationConfigDataManager
    {
        IntegrationConfigResponse Get();
        ResponseBaseDataContract Save(SaveIntegrationConfigRequest request);
        bool IsBenefitApiEnabled();
    }
}

using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IOrcamentoConfigDataManager
    {
        OrcamentoConfigListResponse GetAll();
        ResponseBaseDataContract Save(SaveOrcamentoConfigRequest request);
        ResponseBaseDataContract Deactivate(DeactivateOrcamentoConfigRequest request);
    }
}

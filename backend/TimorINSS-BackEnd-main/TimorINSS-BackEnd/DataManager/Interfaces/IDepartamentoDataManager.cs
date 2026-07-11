using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDepartamentoDataManager
    {
        public SelectDescriptionResponse GetAllDepartamentosAtivo();

        DepartamentoConfigListResponse GetAllConfig();
        DepartamentoConfigResponse SaveConfig(SaveDepartamentoConfigRequest request);
        ResponseBaseDataContract DeactivateConfig(DeactivateDepartamentoConfigRequest request);
    }
}
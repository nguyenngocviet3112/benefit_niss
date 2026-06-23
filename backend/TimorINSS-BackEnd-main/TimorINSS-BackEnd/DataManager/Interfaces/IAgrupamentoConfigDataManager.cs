using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IAgrupamentoConfigDataManager
    {
        public AgrupamentoConfigReponse GetAgrupamentoConfigByIdCodigoContaTipoConta(GetAgrupamentoConfigRequest request);
    }
}
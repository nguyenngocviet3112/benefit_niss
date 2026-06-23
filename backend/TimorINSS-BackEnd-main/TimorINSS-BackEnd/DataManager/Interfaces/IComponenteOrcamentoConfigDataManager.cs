using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteOrcamentoConfigDataManager
    {
        public GetComponenteOrcamentoConfigReponse GetComponenteOrcamentoConfigByTarefaAtivoId(GetComponenteOrcamentoConfigRequest request);
    }
}
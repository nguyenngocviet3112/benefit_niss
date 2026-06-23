using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteDespesaConfigDataManager
    {
        public GetComponenteDespesaConfigReponse GetComponenteDespesaConfigByTarefaAtivoId(GetComponenteDespesaConfigRequest request);
    }
}
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IAgrupamentoRubricaDataManager
    {
        AgrupamentoRubricaTreeResponse GetTreeByOrcamentoConfig(GetAgrupamentoRubricaTreeRequest request);
        ResponseBaseDataContract SaveAgrupamentoRubrica(SaveAgrupamentoRubricaRequest request);
        ResponseBaseDataContract DeactivateAgrupamentoRubrica(DeactivateAgrupamentoRubricaRequest request);
    }
}

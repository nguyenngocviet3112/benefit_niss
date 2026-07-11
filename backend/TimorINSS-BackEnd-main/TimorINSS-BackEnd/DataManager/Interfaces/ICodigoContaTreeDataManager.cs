using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICodigoContaTreeDataManager
    {
        CodigoContaTreeResponse GetTreeByOrcamentoConfig(GetCodigoContaTreeRequest request);
        ResponseBaseDataContract SaveCodigoConta(SaveCodigoContaRequest request);
        ResponseBaseDataContract DeactivateCodigoConta(DeactivateCodigoContaRequest request);
    }
}

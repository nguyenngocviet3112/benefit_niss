using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IEconomicClassificationDataManager
    {
        EconomicClassificationTreeResponse GetTreeByOrcamentoConfig(GetEconomicClassificationTreeRequest request);
        ResponseBaseDataContract SaveEconomicClassification(SaveEconomicClassificationRequest request);
        ResponseBaseDataContract DeactivateEconomicClassification(DeactivateEconomicClassificationRequest request);
        ImportMasterDataTreeResponse ImportEconomicClassification(ImportMasterDataTreeRequest request);
    }
}

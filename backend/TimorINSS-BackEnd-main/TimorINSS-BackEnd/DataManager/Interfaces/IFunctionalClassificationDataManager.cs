using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IFunctionalClassificationDataManager
    {
        FunctionalClassificationTreeResponse GetAllActive();
        ResponseBaseDataContract SaveFunctionalClassification(SaveFunctionalClassificationRequest request);
        ResponseBaseDataContract DeactivateFunctionalClassification(DeactivateFunctionalClassificationRequest request);
        ImportMasterDataTreeResponse ImportFunctionalClassification(ImportMasterDataTreeRequest request);
    }
}

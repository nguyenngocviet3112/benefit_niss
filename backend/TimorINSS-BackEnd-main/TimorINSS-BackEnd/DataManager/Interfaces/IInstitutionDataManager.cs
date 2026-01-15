using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IInstitutionDataManager
    {
        
        public SelectDescriptionResponse GetAllInstitutionAtivo();
    }
}
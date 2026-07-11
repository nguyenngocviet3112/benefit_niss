using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ILanguageConfigDataManager
    {
        LanguageConfigListResponse GetAll();
        LanguageConfigListResponse GetAllActive();
        ResponseBaseDataContract Toggle(ToggleLanguageConfigRequest request);
    }
}

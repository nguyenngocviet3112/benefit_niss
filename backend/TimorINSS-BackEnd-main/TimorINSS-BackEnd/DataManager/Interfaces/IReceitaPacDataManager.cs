using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IReceitaPacDataManager
    {
        ReceitaPacListResponse GetByAno(GetReceitaPacListRequest request);
        ReceitaPacResponse Save(SaveReceitaPacRequest request);
        ResponseBaseDataContract Deactivate(DeactivateReceitaPacRequest request);
    }
}

using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICicloDespesaDataManager
    {
        CicloDespesaListResponse GetByAno(GetCicloDespesaListRequest request);
    }
}

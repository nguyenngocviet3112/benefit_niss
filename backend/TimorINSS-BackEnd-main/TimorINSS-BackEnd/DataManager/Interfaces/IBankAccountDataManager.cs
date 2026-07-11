using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IBankAccountDataManager
    {
        BankAccountListResponse GetAll();
        SaveBankAccountResponse Save(SaveBankAccountRequest request);
    }
}

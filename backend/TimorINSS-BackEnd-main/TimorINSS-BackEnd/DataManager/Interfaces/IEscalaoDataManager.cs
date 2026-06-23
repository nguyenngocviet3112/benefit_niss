using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IEscalaoDataManager
    {
        public SelectDescriptionResponse GetAllEscaloes();
    }
}
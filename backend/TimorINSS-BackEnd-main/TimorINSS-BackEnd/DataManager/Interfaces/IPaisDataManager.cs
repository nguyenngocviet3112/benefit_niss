using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IPaisDataManager
    {
        public PaisDto Get(int id);

        public SelectDescriptionResponse getPais();
    }
}
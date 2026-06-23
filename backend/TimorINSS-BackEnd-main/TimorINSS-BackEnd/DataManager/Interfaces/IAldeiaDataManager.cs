using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IAldeiaDataManager
    {
        public SelectDescriptionResponse getAllAldeia();

        public AldeiaDto GetDto(int id);

        public SelectDescriptionResponse getAldeiaByIdSuco(int id);
    }
}
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ISectorActividadeDataManager
    {
        public SelectDescriptionResponse GetAllSectorActividade();

        public SectoractividadeDto GetDto(int id);
    }
}
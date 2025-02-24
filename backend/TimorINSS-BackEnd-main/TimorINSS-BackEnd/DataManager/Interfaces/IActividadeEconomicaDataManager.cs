using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IActividadeEconomicaDataManager
    {
        public SelectDescriptionResponse GetAllActividadeEconomica();

        public ActividadeeconomicaDto GetDto(int id);
    }
}
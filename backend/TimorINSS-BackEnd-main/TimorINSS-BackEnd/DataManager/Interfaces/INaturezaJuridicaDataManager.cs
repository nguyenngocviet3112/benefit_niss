using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface INaturezaJuridicaDataManager
    {
        public SelectDescriptionResponse GetAllNaturezaJuridica();

        public NaturezajuridicaDto GetDto(int id);
    }
}
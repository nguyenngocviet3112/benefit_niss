using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDepartamentoDataManager
    {
        public SelectDescriptionResponse GetAllDepartamentosAtivo();

        public SelectDescriptionResponse GetAllInstitutionAtivo();
    }
}
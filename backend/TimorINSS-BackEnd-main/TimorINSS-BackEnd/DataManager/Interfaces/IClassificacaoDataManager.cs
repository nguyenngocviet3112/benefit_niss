using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IClassificacaoDataManager
    {
        public SelectDescriptionResponse GetAllClassificacao();
    }
}
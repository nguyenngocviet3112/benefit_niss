using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelUtilizadorDepartamentoDataManager
    {
        public DepartamentoListagemResponse GetDepartamentosByUserId(int id);
    }
}
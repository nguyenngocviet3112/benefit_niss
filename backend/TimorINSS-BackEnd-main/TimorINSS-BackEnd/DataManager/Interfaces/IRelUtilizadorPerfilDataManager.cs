using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelUtilizadorPerfilDataManager
    {
        public PerfilListagemResponse GetPerfisByUserId(int id);
    }
}
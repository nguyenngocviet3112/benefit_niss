using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IFuncionalidadeDataManager
    {
        public FuncionalidadeListagemResponse GetAllFuncionalidades();
    }
}
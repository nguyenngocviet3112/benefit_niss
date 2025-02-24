using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelPerfilFuncionalidadeDataManager
    {
        public FuncionalidadeListagemResponse GetRelPerfilFuncionalidadeByIdPerfil(int idPerfil);
    }
}
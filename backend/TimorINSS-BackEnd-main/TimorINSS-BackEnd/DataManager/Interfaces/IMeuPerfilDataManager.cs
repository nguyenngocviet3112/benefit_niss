using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IMeuPerfilDataManager
    {
        MeuPerfilResponse GetMeuPerfil(GetMeuPerfilRequest request);
        ResponseBaseDataContract AlterarEmail(AlterarEmailRequest request);
        ResponseBaseDataContract AlterarSenha(AlterarSenhaRequest request);
    }
}

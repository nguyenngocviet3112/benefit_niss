using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IGuiaPagamentoContaConfigDataManager
    {
        GuiaPagamentoContaConfigResponse GetConfig();
        GuiaPagamentoContaConfigResponse SaveConfig(SaveGuiaPagamentoContaConfigRequest request);
    }
}

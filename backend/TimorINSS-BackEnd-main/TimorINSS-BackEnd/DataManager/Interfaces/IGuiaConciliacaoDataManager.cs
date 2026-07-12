using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IGuiaConciliacaoDataManager
    {
        ResponseBaseDataContract ConciliarGuiaPagamento(ConciliarGuiaPagamentoRequest request);
        GuiaComprovativoResponse GetComprovativo(GetGuiaComprovativoRequest request);
    }
}

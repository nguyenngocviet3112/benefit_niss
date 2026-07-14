using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IGuiaConciliacaoDataManager
    {
        BankStatementLineListResponse GetLinhasDisponiveis(SearchFilterRequest request);
        ResponseBaseDataContract ConciliarGuiaPagamento(ConciliarGuiaPagamentoRequest request);
        GuiaComprovativoResponse GetComprovativo(GetGuiaComprovativoRequest request);
        GuiaListagemResponse GetReceitasGpReport(GetReceitasGpReportRequest request);
        ResponseBaseDataContract UndoConciliacao(UndoConciliacaoGuiaRequest request);
    }
}

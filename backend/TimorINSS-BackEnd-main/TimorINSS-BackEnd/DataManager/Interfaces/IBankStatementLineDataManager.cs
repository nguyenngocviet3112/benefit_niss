using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IBankStatementLineDataManager
    {
        BankStatementLineListResponse GetByContaBancaria(GetBankStatementLinesRequest request);
        ReceitasDisponiveisParaConciliacaoResponse GetReceitasDisponiveis(GetReceitasDisponiveisParaConciliacaoRequest request);
        PagamentosDisponiveisParaConciliacaoResponse GetPagamentosDisponiveis();
        ResponseBaseDataContract AddLine(AddBankStatementLineRequest request);
        ResponseBaseDataContract DeleteLine(DeleteBankStatementLineRequest request);
        ResponseBaseDataContract MatchReceita(MatchReceitaRequest request);
        ResponseBaseDataContract MatchPagamento(MatchPagamentoRequest request);
        ResponseBaseDataContract Unmatch(UnmatchRequest request);
    }
}

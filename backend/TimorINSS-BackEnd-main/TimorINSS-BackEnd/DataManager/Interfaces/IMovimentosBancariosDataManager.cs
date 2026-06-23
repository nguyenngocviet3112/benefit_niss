using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IMovimentosBancariosDataManager
    {
        public ResponseBaseDataContract InsertMovimento(MovimentosUpsertDataRequest request);

        public ResponseBaseDataContract UpdateMovimento(MovimentosUpsertDataRequest request);

        public ResponseBaseDataContract DeleteMovimento(MovimentosDeleteRequest request);

        public MovimentosListagemResponse ListMovimentos(MovimentosListagemRequest request);

        public ContasBancariasListagemResponse ListContasBancarias(bool incluirSaldo);

        public DominioDescricaoStringResponse getMovimentoBancarioDropList(int domainFilterId);

        public MovimentosListagemResponse GetListagemConciliacao(MovimentosBancarioConciliacaoListagemRequest request);

        public SaldoMovimentosResponse ListSaldoMovimentos(int? contaId, int? caixaId);

        public ConciliarMovimentosPermissionsListResponse ListPermissions(ConciliarMovimentosPermissionsListRequest request);

        public StringFileReponse ListMovimentosExcel(MovimentosListagemRequest request);

        public ExcelImportReponse ImportMovimentos(ExcelImporterRequest request, int tarefaAtivoId, int? caixaId, int? bancoId);
    }
}
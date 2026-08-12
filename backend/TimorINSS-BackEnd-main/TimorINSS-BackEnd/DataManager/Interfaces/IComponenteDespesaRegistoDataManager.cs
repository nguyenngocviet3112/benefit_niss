using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteDespesaRegistoDataManager
    {
        public ResponseBaseDataContract AddEditComponenteDespesaRegisto(RegistoDespesaRequest request);

        public GetComponenteDespesaRegistoReponse GetAllDespesaRegistadaByTarefaAtivoId(GetAllDespesaRegistadaRequest request);

        public ResponseBaseDataContract DeleteDespesa(DeleteDespesaRequest request);

        public GetValoresDespesaByIdCodigoOrcamentoResponse GetValoresDespesaByIdCodigoOrcamento(GetValoresDespesaByIdCodigoOrcamentoRequest request);

        public ResponseBaseDataContract DeleteAllDespesasRegistadas(DeleteListaDespesaRequest request);

        public ResponseBaseDataContract UpdateDespesa(UpdateDespesaRequest request);

        public GetComponenteDespesaCabimentadaParaExecucaoReponse GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request);

        public GetDespesasRelatoriosReponse GetDespesasRelatorio(GetDespesasRelatorioRequest request);

        public StringFileReponse GetDespesasRelatorioExcel(GetDespesasRelatorioRequest request);

        public GetDespesasCompromissoResponse GetDespesasCompromissoByTarefaAtivoId(GetDespesasCompromissoRequest request);

        public ResponseBaseDataContract DeleteCompromisso(DeleteRequest request);

        public ResponseBaseDataContract UpsertCompromisso(CompromissoUpsertRequest request);

        public GetComponenteDespesaCabimentadaParaExecucaoReponse GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request);
        public ResponseBaseDataContract UpdateDespesaCabimentada(UpdateDespesaCabimentadaRequest request);

        public BudgetExecutionRelatorioResponse GetBudgetExecutionRelatorio(BudgetExecutionRelatorioRequest request);

        public DespesaPipelineRelatorioResponse GetDespesaPipelineRelatorio(DespesaPipelineRelatorioRequest request);
    }
}
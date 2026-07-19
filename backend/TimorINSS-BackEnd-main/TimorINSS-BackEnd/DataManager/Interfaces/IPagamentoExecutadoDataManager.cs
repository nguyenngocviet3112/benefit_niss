using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IPagamentoExecutadoDataManager
    {

        public ResponseBaseDataContract SavePagamentoExecutadoImport(SavePagamentoExecutadoRequest request);
        public ResponseBaseDataContract SavePagamentoExecutado(SavePagamentoExecutadoRequest request);

        public DestinatarioPagamentoExecutadosResponse GetDestinatariosPagamentoByNumPagamento(GetDestinatarioPagamentoRequest request);

        public ResponseBaseDataContract DeletePagamento(DeletePagamentoRequest request);

        public DestinatarioPagamentoExecutadosResponse GetPagamentosExecutadosByIdDestinatario(GetPagamentoExecutadoRequest request);

        public ResponseBaseDataContract EditPagamentoExecutado(EditPagamentoExecutadoRequest request);

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosGrouped(RelatorioPagamentosListagemRequest request);

        public SelectDescriptionResponse GetDropdownContasOGE(RelatorioContaOGEDropdownListagemRequest request);

        public StringFileReponse ExtractToExcelRelatorios(RelatorioPagamentosListagemRequest request);

        public RelatorioPagamentosListagemResponse GetPagamentosRelatorios(RelatorioPagamentosListagemRequest request);

        public SelectDescriptionResponse GetCentrosCusto(SearchFilterRequest request);

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request);
        public ClassificacaoEconomicaExecucaoListagemResponse GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request);

        public StringFileReponse GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request);
        public DestinatarioPagamentoExecutadosResponse GetListaPagamentoExcel(ListagemPagamentosProcessoRequest request);
        public ListagemPagamentosProcessoResponse GetListaPagamentosProcesso(ListagemPagamentosProcessoRequest request);
        public ListagemPagamentosProcessoResponse GetPagamentoDetails(GetDestinatarioPagamentoRequest request);
        public ClassificacaoContabilisticaRelatorios ClassificacaoContabilisticaRelatorios(SearchFilterRequest request);

        public StringFileReponse ClassificacaoContabilisticaRelatoriosExcel(SearchFilterRequest request);
        public FornecedoresRelatorios FornecedoresRelatorios(FornecedoresRelatoriosRequest request);

        public StringFileReponse FornecedoresRelatoriosExcel(FornecedoresRelatoriosRequest request);

        public BalancoRelatorios BalancoRelatorios(BalancoRelatoriosRequest request);

        public StringFileReponse BalancoRelatoriosExcel(BalancoRelatoriosRequest request);

        public ResponseBaseDataContract SaveClassificacaoExecucao(SaveClassificacaoContabilisticaExecucaoRequest request);

    }
}
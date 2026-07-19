using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteReceitaRegistoDataManager
    {
        public ResponseBaseDataContract AddEditComponenteReceitaRegisto(RegistoReceitaRequest request);

        public ComponentesReceitaRegistoResponseDataContract GetComponenteReceitaRegistoByContaOSSId(GetComponenteReceitaRegistoByIdContaOSSRequest request);

        public ResponseBaseDataContract DeleteReceita(DeleteReceitaRequest request);

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request);
        public ClassificacaoEconomicaExecucaoListagemResponse GetExecucaoOrcamentalPorClassificacaoEconomica(RelatorioClassificacaoEconomicaRequest request);

        public StringFileReponse GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request);

        public GetDespesasRelatoriosReponse ReceitasRelatorios(SearchFilterRequest request);
        public StringFileReponse ReceitasRelatoriosExcel(SearchFilterRequest request);

        public GetReceitasNaoConciliadasRelatoriosReponse ReceitasNaoConciliadasRelatorios(ReceitasNaoConciliadasRelatoriosRequest request);
        public StringFileReponse ReceitasNaoConciliadasRelatoriosExcel(ReceitasNaoConciliadasRelatoriosRequest request);
    }
}
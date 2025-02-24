using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IProcessosDataManager
    {
        public ProcessosListagemResponse GetAllProcessos(SearchFilterRequest request);

        public ResponseBaseDataContract SwitchProcessoState(SwitchProcessoStateRequest request);

        public ResponseBaseDataContract CreateProcessoConfig(ProcessoConfigRequest request);

        public ProcessoConfigListagemResponse GetProcessoConfig(ListProcessoConfiRequest request);

        public ResponseBaseDataContract UpdateProcessoConfig(ProcessoConfigRequest request);

        public SelectDescriptionResponse ListIniciarProcessos(RequestBaseDataContract request);

        public ProcessosArquivadosListagemResponse GetAllProcessosArquivados(SearchFilterRequest request);

        public ProcessoDataResponse GetProcessoData(GetProcessoDataRequest request);

        public ComponenteHistoricoTextoListagemResponse GetHistoricoTexto(GetHistoricoTextoProcessoRequest request);

        public ResponseBaseDataContract StartProcess(StartProcessRequest request);

        public RelatorioProcessosListagemResponse GetProcessosRelatorios(RelatorioProcessosListagemRequest request);

        public GetTipoProcessosRelatoriosResponse GetTipoProcessosRelatorios(RequestBaseDataContract request);

        public StringFileReponse ExtractToExcelRelatorios(RelatorioProcessosListagemRequest request);
    }
}
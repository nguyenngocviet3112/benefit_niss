using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteOrcamentoRegistoDataManager
    {
        public GetComponenteOrcamentoRegistoReponse GetComponenteOrcamentoRegisto(GetComponenteOrcamentoRegistoRequest request);

        public UpdateComponenteOrcamentoRegistoDatesResponse UpdateComponenteOrcamentoRegistoDates(UpdateComponenteOrcamentoRegistoDatesRequest request);

        public GetComponenteOrcamentoAprovadoRegistoReponse GetOrcamentoAprovadoDespesaByIdTarefaActivo(GetComponenteOrcamentoRegistoAprovadoRequest request);

        public GetComponenteOrcamentoRegistoReponse RetificarOrcamentoAprovado(UpdateComponenteOrcamentoRegistoDatesRequest request);

        public ResponseBaseDataContract AprovarOrcamento(GetComponenteOrcamentoRegistoAprovadoRequest request);

        public OrcamentoExtractToExcelReponse ExtractToExcel(OrcamentoExtractRequest request);

        public OrcamentoExtractToPDFReponse ExtractToPDF(OrcamentoExtractRequest request);

        public GetComponenteOrcamentoAprovadoRegistoReponse GetOrcamentoAprovadoReceitaByIdTarefaActivo(GetComponenteOrcamentoRegistoAprovadoRequest request);
    }
}
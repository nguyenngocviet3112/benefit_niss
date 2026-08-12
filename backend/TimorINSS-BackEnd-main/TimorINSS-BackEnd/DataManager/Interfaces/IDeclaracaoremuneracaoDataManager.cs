using System;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IDeclaracaoremuneracaoDataManager
    {
        public GetDeclaracaoByEntidadeAndFilterResponse GetDeclaracaoByEntidadeAndFilter(GetDeclaracaoByEntidadeAndFilterRequest request);

        public ResumoDeclaracaoResponse GetResumoDeclaracao(GetResumoDeclaracaoRequest request);

        public ResponseBaseDataContract SubmitDeclaracao(SaveDeclaracoesRequest request);

        public ResponseBaseDataContract SaveDeclaracao(SaveDeclaracoesRequest request, bool oficioso);

        public ResponseBaseDataContract AutoGenerateNextDeclarations(int entidadeId, DateTime beginDate);

        public RelatorioDeclaracaoRenumeracaoListagemResponse GetDeclaracoesRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request);

        public StringFileReponse ExtractToExcelRelatorios(RelatorioDeclaracaoRenumeracaoListagemRequest request);

        public SituacaoContributivaEmpresasRelatorioResponse GetSituacaoContributivaEmpresasRelatorio(SituacaoContributivaEmpresasRelatorioRequest request);

        public ContribuicoesTrendsRelatorioResponse GetContribuicoesTrendsRelatorio(ContribuicoesTrendsRelatorioRequest request);
    }
}
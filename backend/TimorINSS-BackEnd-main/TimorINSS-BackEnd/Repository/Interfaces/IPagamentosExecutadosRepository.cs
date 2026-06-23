using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IPagamentosExecutadosRepository : IDataRepository<Pagamentosexecutados, PagamentosExecutadosDto>
    {
        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdDespesa(int idDespesa);

        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByNumPagamento(string numPagamento);

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdProcessoAtivo(int idProcessoAtivo, bool includeConciliadosRel = false);

        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByIdDestinatario(int idDestinatario);

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosGrouped(RelatorioPagamentosListagemRequest request);

        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdDespesaIdContaOSS(int idDespesa, int idContaOSS);

        public SelectDescriptionResponse GetDropdownContasOGE(RelatorioContaOGEDropdownListagemRequest request);

        public RelatorioPagamentosListagemResponse GetPagamentosRelatoriosExcel(RelatorioPagamentosListagemRequest request);

        public RelatorioPagamentosListagemResponse GetPagamentosRelatorios(RelatorioPagamentosListagemRequest request);

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request);
        public List<Pagamentosexecutados> GetPagamentosexecutadosByIdProcessoAtivoEstado(int idProcessoAtivo, int estado);
        public List<PagamentoExecutadoDestinatrioDataContract> GetPagamentosexecutadosByProcessoAtivoID(int processoAtivoId);
        public List<ListaPagamentosDoProcessoDataContract> GetListaPagamentosByProcessoAtivoID(int processoAtivoId);
        public List<ListaPagamentosDoProcessoDataContract> GetListaPagamentoByNumPagamento(string numPagamento);

        public ClassificacaoContabilisticaRelatorios ClassificacaoContabilisticaRelatorios(SearchFilterRequest request);

        public string ClassificacaoContabilisticaRelatoriosExcel(SearchFilterRequest request, List<RelatorioClassificacaoContabilisticaDataContract> lista);

        public FornecedoresRelatorios FornecedoresRelatorios(FornecedoresRelatoriosRequest request);

        public string FornecedoresRelatoriosExcel(FornecedoresRelatoriosRequest request, List<RelatorioFornecedoresDataContract> lista);

        public BalancoRelatorios BalancoRelatorios(BalancoRelatoriosRequest request);

        public string BalancoRelatoriosExcel(BalancoRelatoriosRequest request, List<RelatorioBalancoDataContract> lista);

        public List<Pagamentosexecutados> GetPagamentosexecutadosById(List<int> ids);

    }
}
using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteReceitaRegistoRepository : IDataRepository<ComponentereceitaRegisto, ComponenteReceitaRegistoDto>
    {
        public ComponentesReceitaRegistoResponseDataContract GetComponenteReceitaRegistoByContaOSSId(GetComponenteReceitaRegistoByIdContaOSSRequest request);

        public ExecucaoOrcamentalListagemResponse GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request);

        public List<ComponentereceitaRegisto> GetComponenteReceitaRegistoByIdOrcamento(int idOrcamentoRegisto, DateTime dataInicio, DateTime dataFim);

        public GetDespesasRelatoriosReponse ReceitasRelatorios(SearchFilterRequest request);

        public string ReceitasRelatoriosExcel(SearchFilterRequest request, List<DespesasRelatoriosDataContract> lista);

        public GetReceitasNaoConciliadasRelatoriosReponse ReceitasNaoConciliadasRelatorios(ReceitasNaoConciliadasRelatoriosRequest request);

        public string ReceitasNaoConciliadasRelatoriosExcel(ReceitasNaoConciliadasRelatoriosRequest request, List<ReceitasNaoConciliadasRelatorios> lista);
    }
}
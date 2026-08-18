using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IComponenteDespesaRegistoRepository : IDataRepository<ComponentedespesaRegisto, ComponenteDespesaRegistoDto>
    {
        public List<DespesaRegistadaDataContract> GetAllDespesaRegistadaByTarefaAtivoId(int tarefaAtivo);

        public List<ComponentedespesaRegisto> GetAllDespesaRegistadaByAgrupamentoConfigFk(int agrupamentoConfigFk);
        public List<ComponentedespesaRegisto> GetAllDespesaRegistadaByAgrupamentoConfigFk(int agrupamentoConfigFk, int institutionId, int actidadeId,  int funcionalId, int centroCustoId, int componenteOrcamentoRegistoFk);

        public List<DespesaEmCursoDataContract> GetDespesasEmCursoByChave(int agrupamentoConfigFk, int institutionId, int actidadeId, int funcionalId, int centroCustoId, int componenteOrcamentoRegistoFk, List<int> estados, int idExcluir);

        public List<DespesaCabimentadasParaExecucaoDataContract> GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(int processoId, int estado);

        public List<ComponentedespesaRegisto> GetComponenteDespesaRegistoByIdOrcamento(int idOrcamentoRegisto, DateTime dataInicio, DateTime dataFim);

        public GetDespesasRelatoriosReponse GetDespesasRelatorio(GetDespesasRelatorioRequest request, List<int> estadosDespesa, int? estadoPagamento, EstadoDespesaEnum tipo);

        public string GetDespesasRelatorioExcel(GetDespesasRelatorioRequest request, List<DespesasRelatoriosDataContract> lista);

        public List<DespesaCompromissoDataContract> GetAllDespesaCompromissoByTarefaAtivoId(int tarefaAtivo);
        public List<DespesaCabimentadasParaExecucaoDataContract> GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(int processoId);
    }
}
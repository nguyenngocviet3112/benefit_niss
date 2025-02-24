using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IAgrupamentoConfigRepository : IDataRepository<Agrupamentoconfig, AgrupamentoConfigDto>
    {
        public ValueCampoEditavelListagemResponse GetAllActiveAgrupamentoConfig(SearchFilter filter);

        public ValueCampoEditavelListagemResponse GetAllActiveSubAgrupamentoConfig(SearchFilter filter);

        public List<SelectDescription> GetAllAgrupamentoConfigByParent(int parent);

        public List<SelectDescription> GetAllSubAgrupamentoConfigByParent(int parent);

        public List<MultipleSelectAdditionalParameter> GetAllAgrupamentosMultipleSelect(int orcamentoConfigId);

        public bool IsCodeValid(Agrupamentoconfig agrupamento);

        public List<AgrupamentoConfigDataContract> GetAlllActivAgrupamentoConfigByOrcamentoConfig(int orcamentoId);

        public string GetFullCodigo(int id);

        public string GetFullDesignacao(int id);

        public bool IsAgrupamentoDeleteValid(Agrupamentoconfig agrupamento);

        public bool AgrupamentoHasChilds(Agrupamentoconfig agrupamento);

        public string ExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request, List<ExecucaoOrcamentalDataContract> lista);
        public List<AgrupamentoConfigDataContract> GetAlllActivAgrupamentoConfigByOrcamentoConfigTipoConta(int? orcamentoId, int tipoContaFK);

    }
}
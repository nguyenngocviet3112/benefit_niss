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

        public List<AgrupamentoConfigDataContract> GetActidadesAgrupamentoConfigByOrcamentoConfig(int orcamentoId, int tipoContaId);

        public string GetFullCodigo(int id);

        public string GetFullDesignacao(int id);

        public bool IsAgrupamentoDeleteValid(Agrupamentoconfig agrupamento);

        public bool AgrupamentoHasChilds(Agrupamentoconfig agrupamento);

        public string ExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request, List<ExecucaoOrcamentalDataContract> lista);
        public List<AgrupamentoConfigDataContract> GetAlllActivAgrupamentoConfigByOrcamentoConfigTipoConta(int? orcamentoId, int tipoContaFK);

        // Thêm 2026-07-12 cho màn "Mapeamento Rubricas" (mode mới) — chỉ phục vụ 4 loại
        // TipoConta Receita/Despesa/Neutro Receita/Neutro Despesa (mapping Balanço/DR),
        // KHÔNG đụng tới Actidade/Funcional (dữ liệu cũ trùng lặp ProgramActivity/
        // FunctionalClassification — xem AgrupamentoRubricaController).
        public List<Agrupamentoconfig> GetRubricaTreeByOrcamentoConfig(int orcamentoConfigFk, string tipoConta);

        public int GetOrCreateReltipoDeContaOrcamentoConfig(int orcamentoConfigFk, string tipoConta, int userId);

        public bool IsRubricaCodeValid(Agrupamentoconfig agrupamento, int reltipoDeContaOrcamentoConfigFk);
    }
}
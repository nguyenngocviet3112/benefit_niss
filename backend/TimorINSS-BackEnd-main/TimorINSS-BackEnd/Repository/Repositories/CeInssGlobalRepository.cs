using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // CE_INSS_Global — global Receita+Despesa summary grouped only by Classificação Económica
    // (mã kinh tế), no Programa/Atividade/Regime breakdown. Verified against the source workbook's
    // CE_INSS_Global sheet: RECEITAS (4xx) followed by DESPESAS (5xx) in one report. See finance-brd §6.15.
    //
    // Code catalog = EconomicClassification (M1, commit 35d4c6d). Root rows = ParentFk == null &&
    // Tipo in (Receita,Despesa) — the "4"/"RECEITAS" and "5"/"DESPESAS" rows are decorative headers
    // with Tipo == null and are skipped by that filter.
    //
    // Orçamento values = OrcamentoLinha.Valor (M2, commit 2819913), summed per EconomicClassification
    // code (rolled up across each root's descendants) and per Organization when filtered, restricted to
    // batches with Estado == "APPROVED" (DRAFT/PENDING_* are not official budget yet). There is no
    // separate "inicial" vs "corrigido" figure yet (Suplementar adjustments aren't built) — both columns
    // show the same approved total until that feature exists.
    public class CeInssGlobalRepository : ICeInssGlobalRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _context;

        public CeInssGlobalRepository(TimorINSSModuloContribuicoesContext context)
        {
            _context = context;
        }

        public CeInssGlobalResponse GetReport(CeInssGlobalRequest request)
        {
            var response = new CeInssGlobalResponse
            {
                organizationLabel = "INSS Global"
            };

            if (request.institution.HasValue)
            {
                var institution = _context.Institution.SingleOrDefault(i => i.Id == request.institution.Value);
                if (institution != null)
                    response.organizationLabel = institution.Nome;
            }

            var orcamentoConfigFk = _context.Orcamentoconfig
                .Where(o => o.IndActivo && o.DataInicio.Year <= request.year && (o.DataFim == null || o.DataFim.Value.Year >= request.year))
                .Select(o => o.Id)
                .FirstOrDefault();

            if (orcamentoConfigFk == 0)
                return response; // no budget period configured for this year yet

            // Load the FULL flat catalog once and build parent->children in memory, rather than a fixed-
            // depth .Include() chain — the real tree goes at least 5 levels deep (e.g. "503.11.01.02.02"),
            // and a depth-limited Include silently drops deeper leaves (found via live-data verification:
            // 39,526,303 of the real 170,399,205 approved total was missing until this was fixed).
            var allCodes = _context.EconomicClassification
                .Where(e => e.IndActivo && e.OrcamentoConfigFk == orcamentoConfigFk)
                .ToList();
            var childrenByParentId = allCodes
                .Where(e => e.ParentFk.HasValue)
                .GroupBy(e => e.ParentFk.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var roots = allCodes
                .Where(e => e.ParentFk == null && (e.Tipo == "Receita" || e.Tipo == "Despesa"))
                .OrderBy(e => e.Codigo)
                .ToList();

            var valorAprovadoPorCodigo = _context.OrcamentoLinha
                .Where(l => l.IndActivo
                    && l.OrcamentoBatchFkNavigation.Estado == "APPROVED"
                    && l.OrcamentoBatchFkNavigation.OrcamentoConfigFk == orcamentoConfigFk
                    && (!request.institution.HasValue || l.OrganizationFk == request.institution))
                .GroupBy(l => l.EconomicClassificationFk)
                .Select(g => new { EconomicClassificationFk = g.Key, Total = g.Sum(l => l.Valor) })
                .ToList()
                .ToDictionary(g => g.EconomicClassificationFk, g => g.Total);

            foreach (var root in roots)
            {
                var codeIds = GetSelfAndDescendants(root, childrenByParentId).Select(n => n.Id).ToList();
                var valor = codeIds.Sum(id => valorAprovadoPorCodigo.TryGetValue(id, out var v) ? v : 0);

                if (root.Tipo == "Receita")
                {
                    response.receitas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = valor,
                        valorOrcamentado = valor,
                        saldoExecucao = valor // totalExecucao still 0 pending Receita entry (M2 continued)
                    });
                }
                else
                {
                    response.despesas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = valor,
                        valorOrcamentado = valor,
                        saldoExecucao = valor // Cabimentos/Compromissos/Execução still 0 pending M3 (AD/Compromisso/Pagamento)
                    });
                }
            }

            response.totalReceitaInicial = response.receitas.Sum(r => r.valorOrcamentoInicial);
            response.totalReceitaCorrigido = response.receitas.Sum(r => r.valorOrcamentado);
            response.totalDespesaInicial = response.despesas.Sum(d => d.valorOrcamentoInicial);
            response.totalDespesaCorrigido = response.despesas.Sum(d => d.valorOrcamentado);
            response.saldoOrcamental = response.totalReceitaCorrigido - response.totalDespesaCorrigido;

            return response;
        }

        // Self + every descendant node, walked via an in-memory parent->children map (no depth limit).
        private static IEnumerable<EconomicClassification> GetSelfAndDescendants(
            EconomicClassification node,
            Dictionary<int, List<EconomicClassification>> childrenByParentId)
        {
            yield return node;

            if (!childrenByParentId.TryGetValue(node.Id, out var children))
                yield break;

            foreach (var child in children)
                foreach (var descendant in GetSelfAndDescendants(child, childrenByParentId))
                    yield return descendant;
        }
    }
}

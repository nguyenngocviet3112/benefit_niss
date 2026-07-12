using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // CE_OSS_Global — global Receita+Despesa summary grouped only by Classificação Económica
    // (mã kinh tế), no Programa/Atividade/Regime breakdown. Verified against the client's
    // OSS_Global_2026_FINAL_livro.xlsx workbook (sheet literally named "CE_OSS_Global", header
    // "OSS GLOBAL"): RECEITAS (4xx) followed by DESPESAS (5xx) in one report. See finance-brd §6.15
    // and memory oss-global-2026-final-livro-findings (2026-07-11 rename from the earlier
    // inference-based "INSS Global" — the user confirmed: follow the authoritative source, not the
    // earlier guess, see memory legal-source-overrides-inference).
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
    //
    // OSS perimeter (2026-07-11): OrcamentoLinha rows whose Atividade rolls up to a ProgramActivity root
    // with IsOssPerimeter == false (e.g. A08 Regime Contributivo de Capitalização) are EXCLUDED from this
    // report's totals — confirmed with the user: A08 stays fully in the system/master data and gets its
    // own future report, it just isn't part of the OSS Global sum, mirroring the source workbook's own
    // "NÃO INTEGRA O PERÍMETRO DO OSS" label and its separate Prog_CapitalizacaoFRSS sheet.
    // NOT yet implemented: the manual consolidation-adjustment lines ("a deduzir à transferência do
    // Estado...", "a transitar para FRSS...") the source workbook shows between "OSS total" and "OSS
    // total consolidado" — those are per-period manual entries by the finance team, not a fixed formula,
    // and need their own data-entry mechanism before they can be modeled here.
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
                organizationLabel = "OSS Global"
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

            var offPerimeterAtividadeIds = GetOffPerimeterAtividadeIds(orcamentoConfigFk);

            var valorAprovadoPorCodigo = _context.OrcamentoLinha
                .Where(l => l.IndActivo
                    && l.OrcamentoBatchFkNavigation.Estado == "APPROVED"
                    && l.OrcamentoBatchFkNavigation.OrcamentoConfigFk == orcamentoConfigFk
                    && (!request.institution.HasValue || l.OrganizationFk == request.institution))
                .Select(l => new { l.EconomicClassificationFk, l.AtividadeFk, l.Valor })
                .ToList()
                .Where(l => !offPerimeterAtividadeIds.Contains(l.AtividadeFk))
                .GroupBy(l => l.EconomicClassificationFk)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Valor));

            // Receita (PAC) — execution-only, no approval gate (see memory receita-approval-decision),
            // so every active row counts directly (no Estado/APPROVED filter like OrcamentoLinha above).
            // totalExecucao = actual CASH collected (ValorCobradoBanco+ValorCobradoCaixa), mirroring
            // Despesa's "Total Execução (paid)" meaning — NOT ValorPac ("Receita Liquidada", the billed/
            // assessed figure), so saldoExecucao = Orçamentado - Execução stays the same formula on both
            // Receita and Despesa sides (matches the source workbook's "(7) Saldo Execução = (2)-(5)",
            // where (5) is the collected/monthly-breakdown total, not the separate "(3) Receita Liquidada"
            // column). RegimeFk is itself a ProgramActivity node (root or descendant) — reuse
            // offPerimeterAtividadeIds to exclude A08 the same way Despesa does.
            var valorCobradoPacPorCodigo = _context.ReceitaPac
                .Where(r => r.IndActivo
                    && r.Ano == request.year
                    && (!request.institution.HasValue || r.OrganizationFk == request.institution))
                .Select(r => new { r.EconomicClassificationFk, r.RegimeFk, Cobrado = r.ValorCobradoBanco + r.ValorCobradoCaixa })
                .ToList()
                .Where(r => !offPerimeterAtividadeIds.Contains(r.RegimeFk))
                .GroupBy(r => r.EconomicClassificationFk)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Cobrado));

            // Receita GP (Contribuições) — this money lives in the OLD Contribuições module
            // (Contacorrente/Entidadeempregadora/Guiapagamento), which has NO EconomicClassificationFk of
            // its own. Bridged to the 4 leaf codes 401.03.01-04 (Contribuição EE/Trabalhador x Público/
            // Privado) via a mapping confirmed with the user 2026-07-11 — see GetValorCobradoGpPorCodigo.
            var valorCobradoGpPorCodigo = GetValorCobradoGpPorCodigo(request.year, orcamentoConfigFk);

            // Despesa side (2026-07-12) — M3 (AD/Cabimento/Compromisso/Obligation/Pagamento) is now
            // built, so Cabimentos/Compromissos/Execução can be wired for real instead of the earlier
            // placeholder (saldoExecucao hardcoded = valor). Each stage counts only its own APPROVED
            // rows for the given year (mirrors Orçamento only counting APPROVED batches and Receita GP
            // only counting IndPago==1 — "not yet officially final" doesn't count as executed here
            // either), traced back to Classificação Económica via each stage's own chain up to
            // OrcamentoLinha. See GetCabimentosPorCodigo/GetCompromissosPorCodigo/
            // GetExecucaoDespesaPorCodigo below for the exact join paths.
            var cabimentosPorCodigo = GetCabimentosPorCodigo(request.year, offPerimeterAtividadeIds, request.institution);
            var compromissosPorCodigo = GetCompromissosPorCodigo(request.year, offPerimeterAtividadeIds, request.institution);
            var valorExecutadoDespesaPorCodigo = GetExecucaoDespesaPorCodigo(request.year, offPerimeterAtividadeIds, request.institution);

            foreach (var root in roots)
            {
                var codeIds = GetSelfAndDescendants(root, childrenByParentId).Select(n => n.Id).ToList();
                var valor = codeIds.Sum(id => valorAprovadoPorCodigo.TryGetValue(id, out var v) ? v : 0);

                if (root.Tipo == "Receita")
                {
                    var cobrado = codeIds.Sum(id => valorCobradoPacPorCodigo.TryGetValue(id, out var v) ? v : 0)
                        + codeIds.Sum(id => valorCobradoGpPorCodigo.TryGetValue(id, out var v) ? v : 0);
                    response.receitas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = valor,
                        valorOrcamentado = valor,
                        totalExecucao = cobrado,
                        taxaExecucao = valor == 0 ? 0 : cobrado / valor,
                        saldoExecucao = valor - cobrado
                    });
                }
                else
                {
                    var cabimentos = codeIds.Sum(id => cabimentosPorCodigo.TryGetValue(id, out var v) ? v : 0);
                    var compromissos = codeIds.Sum(id => compromissosPorCodigo.TryGetValue(id, out var v) ? v : 0);
                    var executado = codeIds.Sum(id => valorExecutadoDespesaPorCodigo.TryGetValue(id, out var v) ? v : 0);

                    response.despesas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = valor,
                        valorOrcamentado = valor,
                        cabimentos = cabimentos,
                        compromissos = compromissos,
                        totalExecucao = executado,
                        taxaExecucao = valor == 0 ? 0 : executado / valor,
                        saldoExecucao = valor - executado,
                        saldoComprometidoNaoLiquidado = compromissos - executado,
                        saldoCabimentadoNaoComprometido = cabimentos - compromissos
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

        // Receita GP (Contribuições) collected amount, bridged from the OLD Contribuições module into
        // the 4 leaf Classificação Económica codes it maps to. Confirmed with the user 2026-07-11:
        //   - "Público" = Entidadeempregadora.EntidadeNatJuridicaFk in (10, 11) — looked up live in
        //     NATUREZAJURIDICA: 10 = "Empresa Publico", 11 = "Instituição Publico", the only 2 government
        //     codes out of 11 total (01-09 are private-sector legal forms: Lda, SA, ONG, Fundação, etc.).
        //     Everything else = Privado. Spelling this mapping out here (not hiding it behind an opaque
        //     helper/flag elsewhere) is deliberate — the user asked to keep the reasoning traceable.
        //   - "Cobrado" (collected) = Contacorrente.ValorEntidade / ValorTrabalhador, counted only for
        //     Contacorrente rows that have at least one linked Guiapagamento with IndPago == 1 (paid) —
        //     confirmed with the user as the source of truth for "already collected", mirroring
        //     ReceitaPac's ValorCobradoBanco+ValorCobradoCaixa on the PAC side.
        //   - Codigo mapping (verified against the real RECEITAS_GP sheet's row descriptions):
        //     401.03.01 = Contribuição EE Setor Público   ("6% EE públicas")
        //     401.03.02 = Contribuição EE Setor Privado    ("6% EE setor privado")
        //     401.03.03 = Cotização Trabalhador Setor Público  ("4% trabalhadores Estado")
        //     401.03.04 = Cotização Trabalhador Setor Privado  ("4% trabalhadores setor privado")
        private Dictionary<int, decimal> GetValorCobradoGpPorCodigo(int year, int orcamentoConfigFk)
        {
            var contas = _context.Contacorrente
                .Where(c => c.IndActivo
                    && c.MesAno.Year == year
                    && c.ContaCorrenteEntidadeFk.HasValue
                    && c.Guiapagamento.Any(g => g.IndPago == 1))
                .Select(c => new
                {
                    c.ValorEntidade,
                    c.ValorTrabalhador,
                    NatJuridicaFk = c.ContaCorrenteEntidadeFkNavigation.EntidadeNatJuridicaFk
                })
                .ToList();

            // Scoped to this budget period's own catalog copy — EconomicClassification.Codigo is only
            // unique WITHIN one OrcamentoConfigFk (see IsCodeValid), so a plain Codigo lookup across all
            // periods would crash ToDictionary on a duplicate key once a 2nd period's catalog exists.
            var codigoIdByCodigo = _context.EconomicClassification
                .Where(e => e.IndActivo && e.OrcamentoConfigFk == orcamentoConfigFk
                    && new[] { "401.03.01", "401.03.02", "401.03.03", "401.03.04" }.Contains(e.Codigo))
                .ToDictionary(e => e.Codigo, e => e.Id);

            var result = new Dictionary<int, decimal>();
            void Add(string codigo, decimal valor)
            {
                if (valor == 0 || !codigoIdByCodigo.TryGetValue(codigo, out var id)) return;
                result[id] = result.TryGetValue(id, out var existing) ? existing + valor : valor;
            }

            foreach (var conta in contas)
            {
                bool publico = conta.NatJuridicaFk == 10 || conta.NatJuridicaFk == 11;
                Add(publico ? "401.03.01" : "401.03.02", conta.ValorEntidade);
                Add(publico ? "401.03.03" : "401.03.04", conta.ValorTrabalhador);
            }

            return result;
        }

        // Cabimentos — sum of Cabimento.ValorCabimentado for APPROVED cabimentos in the given year,
        // traced back to Classificação Económica via ExpenditureAuthorization -> OrcamentoLinha (the
        // same rúbrica the AD/Cabimento was generated from, 1:1 both steps).
        private Dictionary<int, decimal> GetCabimentosPorCodigo(int year, HashSet<int> offPerimeterAtividadeIds, int? institution)
        {
            var cabimentos = _context.Cabimento
                .Include(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation)
                .Where(c => c.IndActivo && c.Ano == year && c.Estado == "APPROVED")
                .ToList();

            var result = new Dictionary<int, decimal>();
            foreach (var c in cabimentos)
            {
                var rubrica = c.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                if (rubrica == null || offPerimeterAtividadeIds.Contains(rubrica.AtividadeFk)) continue;
                if (institution.HasValue && rubrica.OrganizationFk != institution.Value) continue;

                result[rubrica.EconomicClassificationFk] = result.TryGetValue(rubrica.EconomicClassificationFk, out var existing)
                    ? existing + c.ValorCabimentado
                    : c.ValorCabimentado;
            }
            return result;
        }

        // Compromissos — sum of CompromissoDespesa's "Valor revisto" (ValorCompromissoAno +
        // Regularizacao, this year's committed portion) for APPROVED compromissos, traced back via
        // Cabimento -> ExpenditureAuthorization -> OrcamentoLinha (one Cabimento can have MANY
        // Compromissos, e.g. one per staff member on a shared salary line).
        private Dictionary<int, decimal> GetCompromissosPorCodigo(int year, HashSet<int> offPerimeterAtividadeIds, int? institution)
        {
            var compromissos = _context.CompromissoDespesa
                .Include(cp => cp.CabimentoFkNavigation).ThenInclude(c => c.ExpenditureAuthorizationFkNavigation).ThenInclude(a => a.OrcamentoLinhaFkNavigation)
                .Where(cp => cp.IndActivo && cp.Ano == year && cp.Estado == "APPROVED")
                .ToList();

            var result = new Dictionary<int, decimal>();
            foreach (var cp in compromissos)
            {
                var rubrica = cp.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                if (rubrica == null || offPerimeterAtividadeIds.Contains(rubrica.AtividadeFk)) continue;
                if (institution.HasValue && rubrica.OrganizationFk != institution.Value) continue;

                var valorRevisto = cp.ValorCompromissoAno + cp.Regularizacao;
                result[rubrica.EconomicClassificationFk] = result.TryGetValue(rubrica.EconomicClassificationFk, out var existing)
                    ? existing + valorRevisto
                    : valorRevisto;
            }
            return result;
        }

        // Execução (paid) — the actual money-out event. Distributed at ObligationItem granularity
        // (not PaymentAuthorization.ValorAutorizado as a lump sum) because one Obligation can group
        // Compromissos from DIFFERENT rúbricas/Classificação Económica codes (that's the whole point of
        // Obligation's N:N via ObligationItem) — only counting a PaymentAuthorization whose
        // PaymentExecution actually exists (Realização do Pagamento done, not just Autorização
        // APPROVED) matches "already paid", mirroring Receita's ValorCobradoBanco+Caixa (collected) vs
        // ValorPac (billed) distinction on the other side of the ledger.
        private Dictionary<int, decimal> GetExecucaoDespesaPorCodigo(int year, HashSet<int> offPerimeterAtividadeIds, int? institution)
        {
            var executedAuthorizations = _context.PaymentAuthorization
                .Include(pa => pa.PaymentExecution)
                .Include(pa => pa.ObligationFkNavigation).ThenInclude(o => o.ObligationItem).ThenInclude(oi => oi.CompromissoDespesaFkNavigation).ThenInclude(c => c.CabimentoFkNavigation).ThenInclude(cab => cab.ExpenditureAuthorizationFkNavigation).ThenInclude(ad => ad.OrcamentoLinhaFkNavigation)
                .Where(pa => pa.IndActivo && pa.Ano == year && pa.PaymentExecution != null)
                .ToList();

            var result = new Dictionary<int, decimal>();
            foreach (var pa in executedAuthorizations)
            {
                var items = pa.ObligationFkNavigation?.ObligationItem?.Where(oi => oi.IndActivo) ?? Enumerable.Empty<ObligationItem>();
                foreach (var item in items)
                {
                    var rubrica = item.CompromissoDespesaFkNavigation?.CabimentoFkNavigation?.ExpenditureAuthorizationFkNavigation?.OrcamentoLinhaFkNavigation;
                    if (rubrica == null || offPerimeterAtividadeIds.Contains(rubrica.AtividadeFk)) continue;
                    if (institution.HasValue && rubrica.OrganizationFk != institution.Value) continue;

                    result[rubrica.EconomicClassificationFk] = result.TryGetValue(rubrica.EconomicClassificationFk, out var existing)
                        ? existing + item.Value
                        : item.Value;
                }
            }
            return result;
        }

        // Every ProgramActivity.Id (at any level: Programa/Subprograma/Atividade) that rolls up to a
        // root Programa with IsOssPerimeter == false, for the given budget year.
        private HashSet<int> GetOffPerimeterAtividadeIds(int orcamentoConfigFk)
        {
            var allActivities = _context.ProgramActivity
                .Where(a => a.IndActivo && a.OrcamentoConfigFk == orcamentoConfigFk)
                .ToList();

            var childrenByParentId = allActivities
                .Where(a => a.ParentFk.HasValue)
                .GroupBy(a => a.ParentFk.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var offPerimeterRoots = allActivities.Where(a => a.ParentFk == null && !a.IsOssPerimeter);

            var result = new HashSet<int>();
            foreach (var root in offPerimeterRoots)
                foreach (var node in GetSelfAndDescendants(root, childrenByParentId))
                    result.Add(node.Id);

            return result;
        }

        private static IEnumerable<ProgramActivity> GetSelfAndDescendants(
            ProgramActivity node,
            Dictionary<int, List<ProgramActivity>> childrenByParentId)
        {
            yield return node;

            if (!childrenByParentId.TryGetValue(node.Id, out var children))
                yield break;

            foreach (var child in children)
                foreach (var descendant in GetSelfAndDescendants(child, childrenByParentId))
                    yield return descendant;
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

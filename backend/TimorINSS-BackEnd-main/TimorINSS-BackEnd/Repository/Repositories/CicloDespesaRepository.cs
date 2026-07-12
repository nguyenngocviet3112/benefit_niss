using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // Ciclo da Despesa — 1 row per AD, tracking the whole execution chain Cabimento ->
    // Compromisso -> Obrigação -> Pagamento with running balances (Saldo1/2/3), mirroring the
    // client's "Ciclo_Despesa" sheet (INSS_2026_janeiro _original.xlsx) column-for-column.
    // Unlike CE_OSS_Global this is the original (pre-OSS-rename) register — no A08/OSS-perimeter
    // exclusion here, every AD in the given year is shown regardless of regime.
    public class CicloDespesaRepository : ICicloDespesaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _context;

        public CicloDespesaRepository(TimorINSSModuloContribuicoesContext context)
        {
            _context = context;
        }

        public List<CicloDespesaDataContract> GetByAno(int ano, int? institution)
        {
            var ads = _context.ExpenditureAuthorization
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.AtividadeFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.EconomicClassificationFkNavigation)
                .Include(a => a.OrcamentoLinhaFkNavigation).ThenInclude(l => l.FunctionalClassificationFkNavigation)
                .Where(a => a.IndActivo && a.Ano == ano
                    && (!institution.HasValue || a.OrcamentoLinhaFkNavigation.OrganizationFk == institution.Value))
                .ToList();

            if (ads.Count == 0)
            {
                return new List<CicloDespesaDataContract>();
            }

            var adIds = ads.Select(a => a.Id).ToList();

            // Cabimento — 1:1 per AD (only APPROVED counts, same convention as CE_OSS_Global).
            var cabimentoByAdId = _context.Cabimento
                .Where(c => c.IndActivo && c.Estado == "APPROVED" && adIds.Contains(c.ExpenditureAuthorizationFk))
                .ToList()
                .ToDictionary(c => c.ExpenditureAuthorizationFk);

            var cabimentoIds = cabimentoByAdId.Values.Select(c => c.Id).ToList();

            // Compromisso — 1:N per Cabimento (only APPROVED counts).
            var compromissosByCabimentoId = _context.CompromissoDespesa
                .Where(cp => cp.IndActivo && cp.Estado == "APPROVED" && cabimentoIds.Contains(cp.CabimentoFk))
                .ToList()
                .GroupBy(cp => cp.CabimentoFk)
                .ToDictionary(g => g.Key, g => g.ToList());

            var compromissoIds = compromissosByCabimentoId.Values.SelectMany(list => list.Select(c => c.Id)).ToList();

            // ObligationItem — this Compromisso's own share of whichever Obrigação it was grouped
            // into (Obrigação is N:N with Compromisso, one combined payment batch can span several).
            var itemsByCompromissoId = _context.ObligationItem
                .Where(oi => oi.IndActivo && compromissoIds.Contains(oi.CompromissoDespesaFk))
                .ToList()
                .GroupBy(oi => oi.CompromissoDespesaFk)
                .ToDictionary(g => g.Key, g => g.ToList());

            var obligationIds = itemsByCompromissoId.Values
                .SelectMany(list => list.Select(oi => oi.ObligationFk))
                .Distinct()
                .ToList();

            // Total item value PER Obrigação (across ALL its Compromissos, not just ours) — needed
            // to prorate a shared Pagamento back down to this Compromisso's own slice.
            var totalItemValueByObligationId = _context.ObligationItem
                .Where(oi => oi.IndActivo && obligationIds.Contains(oi.ObligationFk))
                .ToList()
                .GroupBy(oi => oi.ObligationFk)
                .ToDictionary(g => g.Key, g => g.Sum(oi => oi.Value));

            // Pagamento — only counts once Realização (PaymentExecution) actually happened, not
            // merely Autorização APPROVED (mirrors CE_OSS_Global's GetExecucaoDespesaPorCodigo).
            var paidAmountByObligationId = _context.PaymentAuthorization
                .Include(pa => pa.PaymentExecution)
                .Where(pa => pa.IndActivo && pa.PaymentExecution != null && obligationIds.Contains(pa.ObligationFk))
                .ToList()
                .ToDictionary(pa => pa.ObligationFk, pa => pa.ValorAutorizado);

            var activityById = _context.ProgramActivity
                .Where(a => a.IndActivo)
                .ToDictionary(a => a.Id);

            var result = new List<CicloDespesaDataContract>();

            foreach (var ad in ads)
            {
                var rubrica = ad.OrcamentoLinhaFkNavigation;
                var atividade = rubrica?.AtividadeFkNavigation;
                var regime = atividade != null ? GetRoot(atividade, activityById) : null;

                decimal cabimentoValor = 0;
                decimal compromissoValor = 0;
                decimal obrigacaoValor = 0;
                decimal pagamentoValor = 0;

                if (cabimentoByAdId.TryGetValue(ad.Id, out var cabimento))
                {
                    cabimentoValor = cabimento.ValorCabimentado;

                    if (compromissosByCabimentoId.TryGetValue(cabimento.Id, out var compromissos))
                    {
                        foreach (var compromisso in compromissos)
                        {
                            compromissoValor += compromisso.ValorCompromissoAno + compromisso.Regularizacao;

                            if (!itemsByCompromissoId.TryGetValue(compromisso.Id, out var items))
                            {
                                continue;
                            }

                            foreach (var item in items)
                            {
                                obrigacaoValor += item.Value;

                                bool hasPaid = paidAmountByObligationId.TryGetValue(item.ObligationFk, out var paidTotal);
                                bool hasTotalItems = totalItemValueByObligationId.TryGetValue(item.ObligationFk, out var totalItems);
                                if (hasPaid && hasTotalItems && totalItems != 0)
                                {
                                    pagamentoValor += paidTotal * (item.Value / totalItems);
                                }
                            }
                        }
                    }
                }

                result.Add(new CicloDespesaDataContract
                {
                    adId = ad.Id,
                    numeroAd = ad.Numero,
                    regimeCodigo = regime?.Codigo,
                    regimeDesignacao = regime?.Designacao,
                    atividadeCodigo = atividade?.Codigo,
                    atividadeDesignacao = atividade?.Designacao,
                    classificacaoEconomicaCodigo = rubrica?.EconomicClassificationFkNavigation?.Codigo,
                    classificacaoEconomicaDesignacao = rubrica?.EconomicClassificationFkNavigation?.Designacao,
                    classificacaoFuncionalCodigo = rubrica?.FunctionalClassificationFkNavigation?.Codigo,
                    classificacaoFuncionalDesignacao = rubrica?.FunctionalClassificationFkNavigation?.Designacao,
                    cabimentos = cabimentoValor,
                    compromissos = compromissoValor,
                    saldo1 = cabimentoValor - compromissoValor,
                    obrigacoes = obrigacaoValor,
                    saldo2 = compromissoValor - obrigacaoValor,
                    pagamentos = pagamentoValor,
                    saldo3 = obrigacaoValor - pagamentoValor
                });
            }

            return result
                .OrderBy(r => r.regimeCodigo)
                .ThenBy(r => r.atividadeCodigo)
                .ThenBy(r => r.numeroAd)
                .ToList();
        }

        // Walk ParentFk up to the root Programa (A04/A05/A06/A07/A08) — "Regime" in the report.
        private static ProgramActivity GetRoot(ProgramActivity node, Dictionary<int, ProgramActivity> activityById)
        {
            var current = node;
            while (current.ParentFk.HasValue && activityById.TryGetValue(current.ParentFk.Value, out var parent))
            {
                current = parent;
            }
            return current;
        }
    }
}

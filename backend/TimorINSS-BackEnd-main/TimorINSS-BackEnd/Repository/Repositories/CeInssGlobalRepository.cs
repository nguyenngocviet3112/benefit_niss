using Microsoft.EntityFrameworkCore;
using System;
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
    // Root-level Agrupamentoconfig codes (ParentFk == null) are the report rows; each row's figures
    // are summed across that root and every descendant leaf code, mirroring the GetLeafNodes technique
    // already used by PagamentosExecutadosRepository.GetExecucaoOrcamental.
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

            // Agrupamentoconfig is a SHARED self-ref tree covering several classification dimensions,
            // distinguished by ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation:
            // Dominio "TIPOCONTA" valor 1 = Receita (and "Neutro Receita"), valor 2 = Despesa (and
            // "Neutro Despesa"), valor 0 = non-economic trees (Atividade, Classificação Funcional —
            // verified live in DB 2026-07-11). Only valor 1/2 roots belong in this economic-code report.
            var roots = _context.Agrupamentoconfig
                .Where(e => e.IndActivo && e.ParentFk == null
                    && (e.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Valor == 1
                     || e.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Valor == 2))
                .Include(e => e.ReltipoDeContaOrcamentoConfigFkNavigation).ThenInclude(r => r.TipoContaFkNavigation)
                .Include(e => e.InverseParentFkNavigation).ThenInclude(c => c.InverseParentFkNavigation).ThenInclude(c => c.InverseParentFkNavigation)
                .Include(e => e.ComponentedespesaRegisto).ThenInclude(d => d.Compromisso).ThenInclude(c => c.Pagamentosexecutados)
                .Include(e => e.ComponentereceitaRegisto).ThenInclude(r => r.ComponentereceitaRegistoMovimentos).ThenInclude(m => m.RelMovimentosPorConciliarMovimentos).ThenInclude(rm => rm.MovimentosBancariosFkNavigation)
                .Include(e => e.Componenteorcamentovalor).ThenInclude(v => v.ComponenteOrcamentoRegistoFkNavigation)
                .ToList();

            foreach (var root in roots)
            {
                var leaves = GetSelfAndDescendants(root).ToList();

                bool isReceita = root.ReltipoDeContaOrcamentoConfigFkNavigation.TipoContaFkNavigation.Valor == 1;
                bool isDespesa = !isReceita;

                var (orcamentoInicial, orcamentoCorrigido) = SumOrcamento(leaves, request);

                if (isDespesa)
                {
                    var cabimentos = SumDespesaCabimentos(leaves, request);
                    var compromissos = SumDespesaCompromissos(leaves, request);
                    var executado = SumDespesaExecutado(leaves, request);

                    response.despesas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = orcamentoInicial,
                        valorOrcamentado = orcamentoCorrigido,
                        cabimentos = cabimentos,
                        compromissos = compromissos,
                        totalExecucao = executado,
                        taxaExecucao = orcamentoCorrigido == 0 ? 0 : executado / orcamentoCorrigido,
                        saldoExecucao = orcamentoCorrigido - executado,
                        saldoComprometidoNaoLiquidado = compromissos - executado,
                        saldoCabimentadoNaoComprometido = cabimentos - compromissos
                    });
                }

                if (isReceita)
                {
                    var receitaLiquidada = SumReceitaLiquidada(leaves, request);

                    response.receitas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao,
                        valorOrcamentoInicial = orcamentoInicial,
                        valorOrcamentado = orcamentoCorrigido,
                        totalExecucao = receitaLiquidada,
                        taxaExecucao = orcamentoCorrigido == 0 ? 0 : receitaLiquidada / orcamentoCorrigido,
                        saldoExecucao = orcamentoCorrigido - receitaLiquidada
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

        // Self + every descendant node in the Agrupamentoconfig tree (mirrors PagamentosExecutadosRepository.GetLeafNodes,
        // but returns ALL nodes rooted at `node`, not just the leaves — every level's own direct records must be summed).
        private static IEnumerable<Agrupamentoconfig> GetSelfAndDescendants(Agrupamentoconfig node)
        {
            yield return node;

            if (node.InverseParentFkNavigation == null)
                yield break;

            foreach (var child in node.InverseParentFkNavigation)
                foreach (var descendant in GetSelfAndDescendants(child))
                    yield return descendant;
        }

        private (decimal inicial, decimal corrigido) SumOrcamento(List<Agrupamentoconfig> leaves, CeInssGlobalRequest request)
        {
            var valores = leaves.SelectMany(l => l.Componenteorcamentovalor)
                .Where(v => v.IndActivo
                    && v.ComponenteOrcamentoRegistoFkNavigation.DataInicio.Year <= request.year
                    && v.ComponenteOrcamentoRegistoFkNavigation.DataFim.Year >= request.year
                    && (!request.institution.HasValue || v.InstitutionId == request.institution))
                .ToList();

            var inicial = valores.OrderBy(v => v.DataCriacao).Select(v => v.Valor).FirstOrDefault();
            var corrigido = valores.Where(v => v.ComponenteOrcamentoRegistoFkNavigation.Aprovado)
                .OrderBy(v => v.DataCriacao).Select(v => v.Valor).LastOrDefault();

            return (inicial, corrigido == 0 ? inicial : corrigido);
        }

        private decimal SumDespesaCabimentos(List<Agrupamentoconfig> leaves, CeInssGlobalRequest request)
        {
            return leaves.SelectMany(l => l.ComponentedespesaRegisto)
                .Where(d => d.IndActivo
                    && d.DataCriacao.Year == request.year
                    && (!request.month.HasValue || d.DataCriacao.Month == request.month)
                    && (!request.institution.HasValue || d.InstitutionId == request.institution))
                .Sum(d => d.Valor);
        }

        private decimal SumDespesaCompromissos(List<Agrupamentoconfig> leaves, CeInssGlobalRequest request)
        {
            return leaves.SelectMany(l => l.ComponentedespesaRegisto)
                .Where(d => !request.institution.HasValue || d.InstitutionId == request.institution)
                .SelectMany(d => d.Compromisso)
                .Where(c => c.IndActivo
                    && c.DataCriacao.Year == request.year
                    && (!request.month.HasValue || c.DataCriacao.Month == request.month))
                .Sum(c => c.Valor);
        }

        private decimal SumDespesaExecutado(List<Agrupamentoconfig> leaves, CeInssGlobalRequest request)
        {
            return leaves.SelectMany(l => l.ComponentedespesaRegisto)
                .Where(d => !request.institution.HasValue || d.InstitutionId == request.institution)
                .SelectMany(d => d.Compromisso)
                .SelectMany(c => c.Pagamentosexecutados)
                .Where(p => p.IndActivo
                    && p.DataCriacao.Year == request.year
                    && (!request.month.HasValue || p.DataCriacao.Month == request.month))
                .Sum(p => p.ValorExecutado);
        }

        private decimal SumReceitaLiquidada(List<Agrupamentoconfig> leaves, CeInssGlobalRequest request)
        {
            // NOTE: ComponentereceitaRegisto has no InstitutionId column yet in this branch's schema
            // (Organization/Institution wiring for Receita is in progress on the other session's branch —
            // see memory: parallel-session-worktree-workflow). Institution filter is a no-op on the receita
            // side until that work merges; re-check this when rebasing.
            return leaves.SelectMany(l => l.ComponentereceitaRegisto)
                .Where(r => r.IndActivo)
                .SelectMany(r => r.ComponentereceitaRegistoMovimentos)
                .Where(m => m.IndActivo.GetValueOrDefault()
                    && m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Year == request.year
                    && (!request.month.HasValue || m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.DataValor.Month == request.month))
                .Sum(m => m.RelMovimentosPorConciliarMovimentos.MovimentosBancariosFkNavigation.Credito.GetValueOrDefault());
        }
    }
}

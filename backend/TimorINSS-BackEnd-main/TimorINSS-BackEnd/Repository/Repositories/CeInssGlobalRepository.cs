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
    // Code catalog = the new EconomicClassification table (M1, commit 35d4c6d) — NOT Agrupamentoconfig
    // (the legacy tree, still used by the old ExecucaoOrcamental report) and NOT Codigoconta (SNC-TL
    // bookkeeping chart of accounts, unrelated). Root rows = ParentFk == null && Tipo in (Receita,Despesa)
    // — the "4"/"RECEITAS" and "5"/"DESPESAS" rows are decorative headers with Tipo == null and are
    // skipped by that filter.
    //
    // NOTE (2026-07-11): every figure besides the code/designação is 0 for now — checked live DB, the
    // legacy Componenteorcamentovalor/Agrupamentoconfig tree that actually holds budget values uses an
    // unrelated code scheme (short local numbers like "01"-"04", not economic-code strings), so there is
    // no way to bridge real numbers into the new EconomicClassification catalog yet. This report will
    // start showing real Orçamento/Cabimento/Compromisso/Execução values once the M2/M3 entry screens are
    // rewired to write against EconomicClassification directly — that rewire is the real fix, not this file.
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

            var roots = _context.EconomicClassification
                .Where(e => e.IndActivo && e.OrcamentoConfigFk == orcamentoConfigFk && e.ParentFk == null
                    && (e.Tipo == "Receita" || e.Tipo == "Despesa"))
                .OrderBy(e => e.Codigo)
                .ToList();

            foreach (var root in roots)
            {
                if (root.Tipo == "Receita")
                {
                    response.receitas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao
                    });
                }
                else
                {
                    response.despesas.Add(new CeInssGlobalDataContract
                    {
                        agrupamentoId = root.Id,
                        codigo = root.Codigo,
                        designacao = root.Designacao
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
    }
}

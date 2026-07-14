using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class BankStatementLineGuiaPagamentoRepository : IBankStatementLineGuiaPagamentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public BankStatementLineGuiaPagamentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<BankStatementLine> GetDisponiveisParaGuiaPagamento(DateTime? dataInicio, DateTime? dataFim)
        {
            var matchedLineIds = _moduloContribuicoesContext.BankStatementLineGuiaPagamento
                .Where(r => r.IndActivo)
                .Select(r => r.BankStatementLineFk);

            return _moduloContribuicoesContext.BankStatementLine
                .Where(l => l.IndActivo
                    && l.ReceitaPacFk == null
                    && l.PaymentExecutionFk == null
                    && !matchedLineIds.Contains(l.Id)
                    && (dataInicio == null || l.DataValor >= dataInicio)
                    && (dataFim == null || l.DataValor <= dataFim))
                .OrderByDescending(l => l.DataValor)
                .ToList();
        }

        public bool AnyLineAlreadyMatched(List<int> bankStatementLineIds)
        {
            if (bankStatementLineIds == null || bankStatementLineIds.Count == 0) return false;

            return _moduloContribuicoesContext.BankStatementLineGuiaPagamento
                .Any(r => r.IndActivo && bankStatementLineIds.Contains(r.BankStatementLineFk))
                || _moduloContribuicoesContext.BankStatementLine
                .Any(l => bankStatementLineIds.Contains(l.Id) && (l.ReceitaPacFk != null || l.PaymentExecutionFk != null));
        }

        public bool AnyGuiaAlreadyMatched(List<int> guiaPagamentoIds)
        {
            if (guiaPagamentoIds == null || guiaPagamentoIds.Count == 0) return false;

            return _moduloContribuicoesContext.BankStatementLineGuiaPagamento
                .Any(r => r.IndActivo && guiaPagamentoIds.Contains(r.GuiaPagamentoFk));
        }

        public void AddRelation(BankStatementLineGuiaPagamento entity)
        {
            _moduloContribuicoesContext.BankStatementLineGuiaPagamento.Add(entity);
        }

        public void DeactivateForGuia(int guiaId)
        {
            foreach (var rel in _moduloContribuicoesContext.BankStatementLineGuiaPagamento
                .Where(r => r.IndActivo && r.GuiaPagamentoFk == guiaId))
            {
                rel.IndActivo = false;
            }
        }
    }
}

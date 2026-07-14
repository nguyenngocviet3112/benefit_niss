using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class BankStatementLineRepository : IBankStatementLineRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public BankStatementLineRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        private IQueryable<BankStatementLine> BaseQuery()
        {
            return _moduloContribuicoesContext.BankStatementLine
                .Include(l => l.ContaBancariaFkNavigation)
                .Include(l => l.ReceitaPacFkNavigation)
                .Include(l => l.PaymentExecutionFkNavigation).ThenInclude(e => e.PaymentAuthorizationFkNavigation).ThenInclude(a => a.ObligationFkNavigation);
        }

        public List<BankStatementLine> GetByContaBancaria(int? contaBancariaFk, DateTime? dataInicio, DateTime? dataFim)
        {
            return BaseQuery()
                .Where(l => l.IndActivo
                    && (contaBancariaFk == null || l.ContaBancariaFk == contaBancariaFk)
                    && (dataInicio == null || l.DataValor >= dataInicio)
                    && (dataFim == null || l.DataValor <= dataFim))
                .OrderByDescending(l => l.DataValor)
                .ToList();
        }

        public BankStatementLine Get(int id)
        {
            return BaseQuery().SingleOrDefault(l => l.Id == id);
        }

        public void Add(BankStatementLine entity)
        {
            _moduloContribuicoesContext.BankStatementLine.Add(entity);
        }

        public void Update(BankStatementLine entity)
        {
            BankStatementLine entityToUpdate = _moduloContribuicoesContext.BankStatementLine
                .Single(l => l.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool HasLineForReceita(int receitaPacFk)
        {
            return _moduloContribuicoesContext.BankStatementLine
                .Any(l => l.IndActivo && l.ReceitaPacFk == receitaPacFk);
        }

        public bool HasLineForPaymentExecution(int paymentExecutionFk)
        {
            return _moduloContribuicoesContext.BankStatementLine
                .Any(l => l.IndActivo && l.PaymentExecutionFk == paymentExecutionFk);
        }

        public bool HasDuplicate(int contaBancariaFk, DateTime dataValor, decimal credito, decimal debito, string descricao)
        {
            return _moduloContribuicoesContext.BankStatementLine
                .Any(l => l.IndActivo
                    && l.ContaBancariaFk == contaBancariaFk
                    && l.DataValor == dataValor
                    && l.Credito == credito
                    && l.Debito == debito
                    && l.Descricao == descricao);
        }

        public List<BankStatementLine> GetByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return new List<BankStatementLine>();

            return BaseQuery().Where(l => ids.Contains(l.Id)).ToList();
        }

        public List<decimal> GetValores(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return new List<decimal>();

            return _moduloContribuicoesContext.BankStatementLine
                .Where(l => ids.Contains(l.Id))
                .Select(l => l.Credito != 0 ? l.Credito : l.Debito)
                .ToList();
        }
    }
}

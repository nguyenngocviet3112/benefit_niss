using Microsoft.EntityFrameworkCore;
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

        public List<BankStatementLine> GetByContaBancaria(int contaBancariaFk)
        {
            return BaseQuery()
                .Where(l => l.IndActivo && l.ContaBancariaFk == contaBancariaFk)
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
    }
}

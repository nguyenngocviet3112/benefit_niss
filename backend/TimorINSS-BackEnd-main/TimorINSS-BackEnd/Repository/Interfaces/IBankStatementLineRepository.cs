using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IBankStatementLineRepository
    {
        List<BankStatementLine> GetByContaBancaria(int? contaBancariaFk, DateTime? dataInicio, DateTime? dataFim);
        BankStatementLine Get(int id);
        void Add(BankStatementLine entity);
        void Update(BankStatementLine entity);
        bool HasLineForReceita(int receitaPacFk);
        bool HasLineForPaymentExecution(int paymentExecutionFk);
        bool HasDuplicate(int contaBancariaFk, DateTime dataValor, decimal credito, decimal debito, string descricao);
        List<BankStatementLine> GetByIds(List<int> ids);
        List<decimal> GetValores(List<int> ids);
    }
}

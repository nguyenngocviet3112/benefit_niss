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
    }
}

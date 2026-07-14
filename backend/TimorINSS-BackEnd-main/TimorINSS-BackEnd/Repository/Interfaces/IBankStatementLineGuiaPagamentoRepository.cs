using System;
using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IBankStatementLineGuiaPagamentoRepository
    {
        // Dòng sao kê chưa khớp Guia Pagamento nào (và cũng chưa khớp
        // Receita/Pagamento — 1 dòng chỉ được dùng cho 1 mục đích).
        List<BankStatementLine> GetDisponiveisParaGuiaPagamento(DateTime? dataInicio, DateTime? dataFim);
        bool AnyLineAlreadyMatched(List<int> bankStatementLineIds);
        bool AnyGuiaAlreadyMatched(List<int> guiaPagamentoIds);
        void AddRelation(BankStatementLineGuiaPagamento entity);

        // Hủy đối chiếu (2026-07-14) — tắt active mọi quan hệ N:N của 1 Guia, để dòng sao
        // kê ngân hàng liên quan quay lại "khả dụng" và Guia có thể được đối chiếu lại.
        void DeactivateForGuia(int guiaId);
    }
}

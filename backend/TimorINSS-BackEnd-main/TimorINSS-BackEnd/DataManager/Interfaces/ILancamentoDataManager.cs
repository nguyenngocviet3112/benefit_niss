using System;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ILancamentoDataManager
    {
        LancamentoListResponse GetList(GetLancamentoListRequest request);

        // Chạy trong CÙNG transaction của lời gọi (không tự Commit) — nơi gọi
        // (vd PaymentDataManager.Execute) chịu trách nhiệm Commit chung 1 lần.
        // Bỏ qua lặng lẽ nếu debitoFk/creditoFk null (chưa cấu hình tài khoản
        // kế toán) hoặc đã tồn tại Lançamento active cho origem này.
        void GerarSeChuaCo(string origemTipo, int origemId, DateTime data, int? codigoContaDebitoFk,
            int? codigoContaCreditoFk, decimal valor, string descricao);
    }
}

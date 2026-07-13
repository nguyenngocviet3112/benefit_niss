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
        // KHÔNG còn bỏ qua lặng lẽ khi thiếu cấu hình — trả về Gerado/FaltaConfiguracao
        // để nơi gọi thông báo lên người dùng (2026-07-13, user yêu cầu: mọi bút toán
        // hệ thống tự sinh phải thông báo, để phát hiện sai sót; thiếu cấu hình cũng
        // phải báo rõ thay vì âm thầm bỏ qua).
        LancamentoGerarResult GerarSeChuaCo(string origemTipo, int origemId, DateTime data, int? codigoContaDebitoFk,
            int? codigoContaCreditoFk, decimal valor, string descricao);

        // Hủy (soft) bút toán đã sinh cho 1 origem — dùng khi 1 đối chiếu ngân hàng
        // bị Unmatch, để lần đối chiếu kế tiếp có thể sinh lại bút toán đúng.
        // Cùng transaction với lời gọi, không tự Commit.
        void DesfazerSeExiste(string origemTipo, int origemId);
    }
}

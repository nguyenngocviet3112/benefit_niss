using System.Collections.Generic;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IOrcamentoLinhaRepository
    {
        List<OrcamentoLinha> GetByBatch(int batchId);
        List<OrcamentoLinha> GetApprovedByOrcamentoConfig(int orcamentoConfigFk);
        OrcamentoLinha Get(int id);
        void Add(OrcamentoLinha entity);
        void Update(OrcamentoLinha entity);
        bool IsComboValid(OrcamentoLinha entity);

        // Dùng cho bước preview import (CLAUDE.md §6) — tìm dòng đang active
        // trùng Atividade+EC+Organization trong cùng OrcamentoConfig (mọi
        // batch/trạng thái), trả về cả entity (không chỉ bool) để FE hiện
        // giá trị hiện có, cho phép chọn Ghi đè/Bỏ qua.
        OrcamentoLinha FindExistingCombo(int orcamentoConfigFk, int atividadeFk, int economicClassificationFk, int organizationFk);
    }
}

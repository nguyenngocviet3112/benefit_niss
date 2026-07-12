using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class OrcamentoLinhaRepository : IOrcamentoLinhaRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public OrcamentoLinhaRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public List<OrcamentoLinha> GetByBatch(int batchId)
        {
            return _moduloContribuicoesContext.OrcamentoLinha
                .Include(l => l.AtividadeFkNavigation)
                .Include(l => l.EconomicClassificationFkNavigation)
                .Include(l => l.FunctionalClassificationFkNavigation)
                .Include(l => l.OrganizationFkNavigation)
                .Where(l => l.IndActivo && l.OrcamentoBatchFk == batchId)
                .ToList();
        }

        public List<OrcamentoLinha> GetApprovedByOrcamentoConfig(int orcamentoConfigFk)
        {
            return _moduloContribuicoesContext.OrcamentoLinha
                .Include(l => l.AtividadeFkNavigation)
                .Include(l => l.EconomicClassificationFkNavigation)
                .Include(l => l.OrganizationFkNavigation)
                .Include(l => l.OrcamentoBatchFkNavigation)
                .Where(l => l.IndActivo
                    && l.OrcamentoBatchFkNavigation.OrcamentoConfigFk == orcamentoConfigFk
                    && l.OrcamentoBatchFkNavigation.Estado == "APPROVED")
                .ToList();
        }

        public OrcamentoLinha Get(int id)
        {
            _moduloContribuicoesContext.ChangeTracker.LazyLoadingEnabled = false;
            return _moduloContribuicoesContext.OrcamentoLinha.SingleOrDefault(l => l.Id == id);
        }

        public void Add(OrcamentoLinha entity)
        {
            _moduloContribuicoesContext.OrcamentoLinha.Add(entity);
        }

        public void Update(OrcamentoLinha entity)
        {
            OrcamentoLinha entityToUpdate = _moduloContribuicoesContext.OrcamentoLinha
                .Single(l => l.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }

        public bool IsComboValid(OrcamentoLinha entity)
        {
            // Trùng lặp chỉ tính trong CÙNG năm ngân sách (OrcamentoConfigFk) — cùng 1
            // rúbrica (Atividade + Classificação Económica + Organization) hoàn toàn hợp
            // lệ khi lặp lại ở năm ngân sách khác, đó là bản chất của ngân sách hàng năm.
            // Trước đây check toàn cục (mọi batch/năm), gây lỗi "đã tồn tại" giả khi
            // import lại 1 rúbrica đã có từ năm trước.
            int orcamentoConfigFk = _moduloContribuicoesContext.OrcamentoBatch
                .Where(b => b.Id == entity.OrcamentoBatchFk)
                .Select(b => b.OrcamentoConfigFk)
                .FirstOrDefault();

            int countSameCombo = _moduloContribuicoesContext.OrcamentoLinha
                .Where(l => l.IndActivo
                    && l.AtividadeFk == entity.AtividadeFk
                    && l.EconomicClassificationFk == entity.EconomicClassificationFk
                    && l.OrganizationFk == entity.OrganizationFk
                    && l.Id != entity.Id
                    && l.OrcamentoBatchFkNavigation.OrcamentoConfigFk == orcamentoConfigFk)
                .Count();

            return countSameCombo == 0;
        }

        public OrcamentoLinha FindExistingCombo(int orcamentoConfigFk, int atividadeFk, int economicClassificationFk, int organizationFk)
        {
            return _moduloContribuicoesContext.OrcamentoLinha
                .Include(l => l.OrcamentoBatchFkNavigation)
                .SingleOrDefault(l => l.IndActivo
                    && l.AtividadeFk == atividadeFk
                    && l.EconomicClassificationFk == economicClassificationFk
                    && l.OrganizationFk == organizationFk
                    && l.OrcamentoBatchFkNavigation.OrcamentoConfigFk == orcamentoConfigFk);
        }
    }
}

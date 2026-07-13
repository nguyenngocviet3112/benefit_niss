using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class LancamentoRepository : ILancamentoRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public LancamentoRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public void Add(Lancamento entity)
        {
            _moduloContribuicoesContext.Lancamento.Add(entity);
        }

        public bool ExistsForOrigem(string origemTipo, int origemId)
        {
            return _moduloContribuicoesContext.Lancamento
                .Any(l => l.IndActivo && l.OrigemTipo == origemTipo && l.OrigemId == origemId);
        }

        public Lancamento GetByOrigem(string origemTipo, int origemId)
        {
            return _moduloContribuicoesContext.Lancamento
                .SingleOrDefault(l => l.IndActivo && l.OrigemTipo == origemTipo && l.OrigemId == origemId);
        }

        public List<Lancamento> GetByFilter(int? ano, int? mes, string origemTipo)
        {
            IQueryable<Lancamento> query = _moduloContribuicoesContext.Lancamento
                .Include(l => l.CodigoContaDebitoFkNavigation)
                .Include(l => l.CodigoContaCreditoFkNavigation)
                .Where(l => l.IndActivo);

            if (ano.HasValue)
                query = query.Where(l => l.Data.Year == ano.Value);

            if (mes.HasValue)
                query = query.Where(l => l.Data.Month == mes.Value);

            if (!string.IsNullOrEmpty(origemTipo))
                query = query.Where(l => l.OrigemTipo == origemTipo);

            return query.OrderByDescending(l => l.Data).ThenByDescending(l => l.Id).ToList();
        }

        // Dùng khi 1 đối chiếu (match) bị hủy (Unmatch) — bút toán đã sinh từ lần
        // match trước không còn đúng, phải tắt đi để lần match kế tiếp cho cùng
        // origem có thể sinh bút toán mới (ExistsForOrigem chỉ tính dòng active).
        public void DeactivateForOrigem(string origemTipo, int origemId)
        {
            foreach (Lancamento entity in _moduloContribuicoesContext.Lancamento
                .Where(l => l.IndActivo && l.OrigemTipo == origemTipo && l.OrigemId == origemId))
            {
                entity.IndActivo = false;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    public class LiquidacaoContaConfigRepository : ILiquidacaoContaConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public LiquidacaoContaConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        // 5 dòng cố định (1 dòng/Categoria) — seed sẵn qua migration, không có
        // Add() vì Categoria không được tạo mới từ app, chỉ Update CodigoContaFk.
        public List<LiquidacaoContaConfig> GetAll()
        {
            return _moduloContribuicoesContext.LiquidacaoContaConfig
                .Include(c => c.CodigoContaFkNavigation)
                .Where(c => c.IndActivo)
                .OrderBy(c => c.Categoria)
                .ToList();
        }

        public LiquidacaoContaConfig GetByCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria)) return null;

            return _moduloContribuicoesContext.LiquidacaoContaConfig
                .Include(c => c.CodigoContaFkNavigation)
                .SingleOrDefault(c => c.IndActivo && c.Categoria == categoria);
        }

        public void Update(LiquidacaoContaConfig entity)
        {
            LiquidacaoContaConfig entityToUpdate = _moduloContribuicoesContext.LiquidacaoContaConfig
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

using System.Linq;
using TimorINSSBackEnd.Models;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.Repository.Repositories
{
    // Singleton config (Id always = 1) — bật/tắt các tích hợp API chiều vào
    // (hiện chỉ có Benefit module gọi vào api/benefit + api/benefit-data).
    // Migration 2026-07-13f đã seed sẵn dòng mặc định; GetOrCreateDefault chỉ
    // là fallback phòng hờ.
    public class IntegrationConfigRepository : IIntegrationConfigRepository
    {
        private readonly TimorINSSModuloContribuicoesContext _moduloContribuicoesContext;

        public IntegrationConfigRepository(TimorINSSModuloContribuicoesContext storeContext)
        {
            _moduloContribuicoesContext = storeContext;
        }

        public IntegrationConfig Get()
        {
            var config = _moduloContribuicoesContext.IntegrationConfig.SingleOrDefault(c => c.Id == 1);
            if (config == null)
            {
                config = new IntegrationConfig { Id = 1, BenefitApiEnabled = true };
                _moduloContribuicoesContext.IntegrationConfig.Add(config);
                _moduloContribuicoesContext.SaveChanges();
            }
            return config;
        }

        public void Update(IntegrationConfig entity)
        {
            IntegrationConfig entityToUpdate = _moduloContribuicoesContext.IntegrationConfig
                .Single(c => c.Id == entity.Id);

            entityToUpdate = Utils.UpdateClassWithoutVirtuals(entity, entityToUpdate);
        }
    }
}

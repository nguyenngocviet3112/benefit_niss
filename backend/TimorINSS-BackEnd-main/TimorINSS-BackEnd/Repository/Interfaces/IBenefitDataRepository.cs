using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    // Repository độc lập cho bộ API "api/benefit-data" (BRD Benefit module §18).
    // Không phụ thuộc IUnitOfWork để giữ tính năng này tách biệt/dễ gỡ bỏ khỏi backend
    // chính nếu khách hàng cuối chọn không dùng.
    public interface IBenefitDataRepository
    {
        Entidadeempregadora GetCompanyMasterByNiss(string nissCompany);

        Trabalhador GetWorkerFullByNiss(string niss);

        Documentoidentificacao GetDocumentoById(int idDoc);

        (Trabalhador worker, System.Collections.Generic.List<Relentidadetrabalhador> contratos, System.Collections.Generic.List<Suspensoes> suspensoes) GetContributionHistoryByNiss(string niss);
    }
}

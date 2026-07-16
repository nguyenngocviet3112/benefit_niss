using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    // DataManager độc lập cho bộ API "api/benefit-data" (BRD Benefit module §18).
    public interface IBenefitDataDataManager
    {
        BenefitCompanyMasterResponse GetCompanyMasterByNiss(string nissCompany);

        BenefitWorkerProfileResponse GetWorkerFullByNiss(string niss);

        Documentoidentificacao GetDocumentoById(int idDoc);

        BenefitContributionsHistoryResponse GetContributionHistoryByNiss(string niss);
    }
}

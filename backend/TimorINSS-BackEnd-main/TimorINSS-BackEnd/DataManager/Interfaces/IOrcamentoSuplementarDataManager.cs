using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IOrcamentoSuplementarDataManager
    {
        OrcamentoSuplementarBatchResponse GetActiveBatch(GetActiveOrcamentoSuplementarRequest request);
        RubricasAprovadasParaSuplementarResponse GetRubricasAprovadas(GetRubricasAprovadasParaSuplementarRequest request);
        ResponseBaseDataContract SaveLinha(SaveOrcamentoSuplementarLinhaRequest request);
        ResponseBaseDataContract DeleteLinha(DeleteOrcamentoSuplementarLinhaRequest request);
        ResponseBaseDataContract Submit(SubmitOrcamentoSuplementarRequest request);
        ResponseBaseDataContract Review(ReviewOrcamentoSuplementarRequest request);
        ResponseBaseDataContract Approve(ApproveOrcamentoSuplementarRequest request);
    }
}

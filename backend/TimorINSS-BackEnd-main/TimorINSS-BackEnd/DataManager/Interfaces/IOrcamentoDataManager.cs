using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IOrcamentoDataManager
    {
        OrcamentoBatchResponse GetActiveBatch(GetActiveOrcamentoBatchRequest request);
        ResponseBaseDataContract SaveLinha(SaveOrcamentoLinhaRequest request);
        ResponseBaseDataContract DeleteLinha(DeleteOrcamentoLinhaRequest request);
        ResponseBaseDataContract Submit(SubmitOrcamentoBatchRequest request);
        ResponseBaseDataContract Review(ReviewOrcamentoBatchRequest request);
        ResponseBaseDataContract Approve(ApproveOrcamentoBatchRequest request);
        ImportMasterDataTreeResponse Import(ImportOrcamentoRequest request);
    }
}

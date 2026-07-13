using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IExpenditureAuthorizationDataManager
    {
        ExpenditureAuthorizationListResponse GetByAno(GetExpenditureAuthorizationListRequest request);
        StringFileReponse GetByAnoExcel(GetExpenditureAuthorizationListRequest request);
        AvailableRubricasResponse GetAvailableRubricas(GetAvailableRubricasRequest request);
        ExpenditureAuthorizationResponse Create(CreateExpenditureAuthorizationRequest request);
        ResponseBaseDataContract Save(SaveExpenditureAuthorizationRequest request);
        ResponseBaseDataContract SavePlurianualidade(SavePlurianualidadeRequest request);
        ResponseBaseDataContract DeletePlurianualidade(DeletePlurianualidadeRequest request);
        ResponseBaseDataContract Submit(SubmitExpenditureAuthorizationRequest request);
        ResponseBaseDataContract Review(ReviewExpenditureAuthorizationRequest request);
        ResponseBaseDataContract Approve(ApproveExpenditureAuthorizationRequest request);
    }
}

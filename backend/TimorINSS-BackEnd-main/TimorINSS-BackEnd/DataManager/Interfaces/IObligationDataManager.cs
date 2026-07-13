using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IObligationDataManager
    {
        ObligationListResponse GetByAno(GetObligationListRequest request);
        StringFileReponse GetByAnoExcel(GetObligationListRequest request);
        CompromissosComSaldoResponse GetCompromissosComSaldo(GetCompromissosComSaldoRequest request);
        ObligationResponse Create(CreateObligationRequest request);
        ResponseBaseDataContract AddItem(AddObligationItemRequest request);
        ResponseBaseDataContract RemoveItem(RemoveObligationItemRequest request);
        ResponseBaseDataContract AddBeneficiary(AddObligationBeneficiaryRequest request);
        ResponseBaseDataContract RemoveBeneficiary(RemoveObligationBeneficiaryRequest request);
        ResponseBaseDataContract Submit(SubmitObligationRequest request);
        ResponseBaseDataContract Approve(ApproveObligationRequest request);
    }
}

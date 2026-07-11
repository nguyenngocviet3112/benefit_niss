using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IPaymentDataManager
    {
        PaymentAuthorizationListResponse GetByAno(GetPaymentListRequest request);
        ObligacoesDisponiveisParaPagamentoResponse GetObligacoesDisponiveis(GetObligacoesDisponiveisParaPagamentoRequest request);
        CodigoContaOptionsResponse GetCodigoContaOptions();
        ContaBancariaOptionsResponse GetContaBancariaOptions();
        PaymentAuthorizationResponse Create(CreatePaymentAuthorizationRequest request);
        ResponseBaseDataContract Submit(SubmitPaymentAuthorizationRequest request);
        ResponseBaseDataContract Approve(ApprovePaymentAuthorizationRequest request);
        ResponseBaseDataContract Execute(ExecutePaymentRequest request);
    }
}

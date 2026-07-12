using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICompromissoDespesaDataManager
    {
        CompromissoDespesaListResponse GetByAno(GetCompromissoDespesaListRequest request);
        CabimentosDisponiveisResponse GetCabimentosDisponiveis(GetCabimentosDisponiveisRequest request);
        CompromissoDespesaResponse Create(CreateCompromissoDespesaRequest request);
        ResponseBaseDataContract Save(SaveCompromissoDespesaRequest request);
        ResponseBaseDataContract SavePlurianualidade(SaveCompromissoDespesaPlurianualidadeRequest request);
        ResponseBaseDataContract Submit(SubmitCompromissoDespesaRequest request);
        ResponseBaseDataContract Review(ReviewCompromissoDespesaRequest request);
        ResponseBaseDataContract Approve(ApproveCompromissoDespesaRequest request);
    }
}

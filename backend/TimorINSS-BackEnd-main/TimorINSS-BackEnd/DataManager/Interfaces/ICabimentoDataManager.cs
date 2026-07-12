using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICabimentoDataManager
    {
        CabimentoListResponse GetByAno(GetCabimentoListRequest request);
        AdsDisponiveisParaCabimentoResponse GetAdsDisponiveis(GetAdsDisponiveisParaCabimentoRequest request);
        CabimentoResponse Create(CreateCabimentoRequest request);
        ResponseBaseDataContract Save(SaveCabimentoRequest request);
        ResponseBaseDataContract Submit(SubmitCabimentoRequest request);
        ResponseBaseDataContract Approve(ApproveCabimentoRequest request);
    }
}

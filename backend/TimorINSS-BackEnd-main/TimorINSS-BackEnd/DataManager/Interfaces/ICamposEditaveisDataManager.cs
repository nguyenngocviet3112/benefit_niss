using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ICamposEditaveisDataManager
    {
        public CamposEditaveisListagemResponse GetAllCamposEditaveis(RequestBaseDataContract request);

        public ValueCampoEditavelListagemResponse GetValorCampoEditavel(ValorCamposEditaveisRequest request);

        public ResponseBaseDataContract SaveValorCampoEditavel(ValorCamposEditaveisSaveRequest request);

        public ResponseBaseDataContract DeleteValorCampoEditavel(ValorCamposEditaveisDeleteRequest request);

        public ResponseBaseDataContract SaveRegimeCampoEditavel(RegimeCampoEditaveisDeleteRequest request);
    }
}
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteOrcamentoAjusteDataManager
    {
        public SolicitarAjusteOrcamentoResponse SolicitarAjuste(SolicitarAjusteOrcamentoRequest request);

        public ResponseBaseDataContract AprovarAjuste(AprovarAjusteOrcamentoRequest request);

        public ResponseBaseDataContract RejeitarAjuste(RejeitarAjusteOrcamentoRequest request);

        public GetAjustesOrcamentoResponse GetPendentes(GetAjustesOrcamentoRequest request);

        public GetAjustesOrcamentoResponse GetHistorico(GetAjustesOrcamentoRequest request);

        public GetRubricasDisponiveisResponse GetRubricasDisponiveis(RequestBaseDataContract request);
    }
}

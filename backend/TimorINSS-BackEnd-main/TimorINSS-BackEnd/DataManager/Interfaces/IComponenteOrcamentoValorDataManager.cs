using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteOrcamentoValorDataManager
    {
        public ResponseBaseDataContract AddOrcamentoValor(AddComponenteOrcamentoValorRequest request);

        public SearchComponentesOrcamentoValorResponse SearchOrcamentoValor(SearchComponenteOrcamentoValorRequest request);

        public ResponseBaseDataContract EditOrcamentoValor(AddComponenteOrcamentoValorRequest request);

        public ResponseBaseDataContract EliminarOrcamentoValor(EliminarComponenteOrcamentoValorRequest request);
    }
}
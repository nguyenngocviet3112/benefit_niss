using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ITarefaDataManager
    {
        public TarefaListagemResponse GetAllTarefas(SearchFilterRequest request);

        public SelectDescriptionResponse GetAllTarefaAtivo();

        public ResponseBaseDataContract AddTarefaConfigurada(ConfigurarTarefaRequest request);

        public ComponenteTarefaConfiguradaRespose GetAllComponentesByIdTarefa(int id, RequestBaseDataContract request);

        public ResponseBaseDataContract EditarTarefa(TarefaRequest request);

        public TarefasAtivasListagemResponse GetAllTarefasAtivas(SearchFilterRequest request);

        public ResponseBaseDataContract LockTarefa(SwitchTarefaAtivoRequest request);

        public ResponseBaseDataContract UnlockTarefa(SwitchTarefaAtivoRequest request);

        public ComponenteHistoricoTextoListagemResponse GetHistoricoTexto(GetHistoricoTextoRequest request);

        public SelectDescriptionResponse GetAllTarefasASeguir(GetAllTarefasASeguirRequest request);

        public TarefaDataResponse GetTarefaData(GetTarefaDataRequest request);

        public ResponseBaseDataContract SaveTarefaData(SaveTarefaDataRequest request);

        public ResponseBaseDataContract ArquivarTarefa(SaveTarefaDataRequest request);

        public string GetTituloListaPagamento(int tarefaActivoId);
    }
}
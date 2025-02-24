using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IComponenteDataManager
    {
        public ComponentesListagemResponse GetAllComponentes();

        public ResponseBaseDataContract EditarComponenteTexto(ComponenteTextoRequest request);

        public ResponseBaseDataContract EditarComponentePrazoTarefa(ComponentePrazoTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteAccaoTarefa(ComponenteAccaoTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteDocumentoTarefa(ComponenteDocumentoTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteClassificacaoSubClassifTarefa(ComponenteClassificacaoSubClassificTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteControloAcessoPerfilTarefa(ComponenteControloAcessoPerfilTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteControloAcessoUtilizadorTarefa(ComponenteControloAcessoUtilizadorTarefaRequest request);

        public ResponseBaseDataContract EditarComponenteOrcamento(ComponenteOrcamentoRequest request);

        public ResponseBaseDataContract EditarComponenteDespesa(ComponenteDespesaRequest request);

        public ResponseBaseDataContract EditarComponenteConciliacaoMovimentos(ComponenteConciliacaoMovimentosRequest request);

        public ResponseBaseDataContract EditarComponenteReceita(ComponenteReceitaRequest request);
    }
}
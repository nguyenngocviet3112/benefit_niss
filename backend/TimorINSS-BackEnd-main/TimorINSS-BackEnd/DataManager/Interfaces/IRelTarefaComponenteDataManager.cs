using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelTarefaComponenteDataManager
    {
        public ResponseBaseDataContract UpdateRelTarefaComponente(UpdateRelTarefaComponenteRequest request);

        public ComponentesListagemResponse GetAllRelTarefaComponenteByIdTarefaActivo(GetAllRelTarefaComponenteByIdTarefaActivoRequest request);
    }
}
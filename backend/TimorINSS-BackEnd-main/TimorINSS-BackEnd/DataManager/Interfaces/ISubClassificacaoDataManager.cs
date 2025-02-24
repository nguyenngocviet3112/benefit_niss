using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ISubClassificacaoDataManager
    {
        public SelectDescriptionResponse GetAllSubClassificacao();

        public SelectDescriptionResponse GetAllSubClassificacaoByTarefaAtivaId(GetAllSubClassificacaoByTarefaAtivaIdRequest request);
    }
}
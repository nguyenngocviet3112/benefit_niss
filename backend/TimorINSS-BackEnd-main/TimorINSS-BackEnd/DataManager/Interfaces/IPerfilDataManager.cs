using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IPerfilDataManager
    {
        public PerfilListagemResponse GetAllPerfis(SearchFilterRequest request);

        public ResponseBaseDataContract UpdatePerfil(PerfilRequest request);

        public ResponseBaseDataContract AddPerfil(AddPerfilRequest request);

        public ResponseBaseDataContract EditPerfil(AddPerfilRequest request);

        public SelectDescriptionResponse GetAllPerfisAtivo();
    }
}
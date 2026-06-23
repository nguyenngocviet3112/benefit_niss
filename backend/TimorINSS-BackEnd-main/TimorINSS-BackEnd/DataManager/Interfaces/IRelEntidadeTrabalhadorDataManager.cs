using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IRelEntidadeTrabalhadorDataManager
    {
        public ResponseBaseDataContract SaveRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest relEntidadeTrabalhador);

        public ResponseBaseDataContract DesvincularTrabalhador(DesvincularTrabalhadorRequest request);

        public ResponseBaseDataContract EditRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest request);

        public ResponseBaseDataContract EditRelEntidadeTrabalhadorRegime(RelEntidadeTrabalhadorRegimeRequest request);

        public TrabalhadorViewResponse GetTrabalhadorViewById(TrabalhadorListagemRequest request);
    }
}
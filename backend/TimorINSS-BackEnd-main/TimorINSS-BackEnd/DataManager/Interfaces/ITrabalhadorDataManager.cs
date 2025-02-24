using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface ITrabalhadorDataManager
    {
        public TrabalhadorListagemResponse GetTrabalhadoresByIdEntidadeEmpregadora(TrabalhadorListagemRequest request);

        public VincularTrabalhadorListagemResponse getTrabalhadoresByFilter(SearchFilterRequest request);

        public ResponseBaseDataContract SaveTrabalhador(TrabalhadorRequest trabalhador);

        public SingleTrabalhadorResponse GetById(TrabalhadorListagemRequest request);

        public TrabalhadorListagemResponse GetTrabalhadoresByNiss(TrabalhadorListagemRequest request);

        public TrabalhadorListagemResponse GetSingleByNiss(TrabalhadorListagemNissRequest request);

        public ResponseBaseDataContract EditTrabalhador(EditTrabalhadorRequest request);
    }
}
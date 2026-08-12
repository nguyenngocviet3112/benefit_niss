using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IMovimentosPorConciliarDataManager
    {
        public IEnumerable<Movimentosporconciliar> GetAll();

        public Movimentosporconciliar Get(long id);

        public MovimentosPorConciliarDto GetDto(long id);

        public void Add(Movimentosporconciliar entity);

        public void Update(Movimentosporconciliar entity);

        public void Delete(Movimentosporconciliar entity);

        public MovimentosPorConciliar GetListagem(MovimentosPorConciliarListagemRequest request);

        public MovimentosPorConciliar GetListagemConciliacao(MovimentosPorConciliarConciliacaoListagemRequest request);

        public ResponseBaseDataContract CreateMovimentoPorConciliar(CreateMovimentosPorConciliarRequest request);

        public ResponseBaseDataContract UpdateMovimentoPorConciliar(UpdateMovimentosPorConciliarRequest request);

        public ResponseBaseDataContract ConciliarMovimentos(ConciliarMovimentosRequest request);

        public ResponseBaseDataContract DesfazerConciliacao(DesfazerConciliacaoMovimentosRequest request);

        public MovimentosPorConciliar GetMovimentosConciliados(GetMovimentosConciliadosRequest request);

        public Movimentosporconciliar GetByGuiaOrReserva(int id, bool isGuia);
    }
}
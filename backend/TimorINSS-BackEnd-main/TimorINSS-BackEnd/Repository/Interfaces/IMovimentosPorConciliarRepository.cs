using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IMovimentosPorConciliarRepository : IDataRepository<Movimentosporconciliar, MovimentosPorConciliarDto>
    {
        public MovimentosPorConciliar GetListagem(MovimentosPorConciliarListagemRequest request);
        public RelMovimentosporconciliarMovimentos CreateRelationObject(int movimentoBancario, MovimentosAConciliar movimentoAConciliar, int idDominio);
        public void AddRelation(RelMovimentosporconciliarMovimentos entity);

        public bool TemConciliados(List<MovimentosAConciliar> items);

        public MovimentosPorConciliar GetListagemConciliacao(MovimentosPorConciliarConciliacaoListagemRequest request, int idEstadoPagamentoEmitido);

        public List<decimal> GetValores(List<MovimentosAConciliar> items);

        public MovimentosPorConciliar GetMovimentosConciliados(SearchFilterRequest request, int estado);

        public List<MovimentosPorConciliarListagem> GetExtraDataByListIds(List<int> ids);

        public List<MovimentosPorConciliarListagem> GetMovimentosConciliadosByIdEstado(IEnumerable<int> ids, int estado);

        public Movimentosporconciliar GuiaExistenteNosMovimentos(int idGuia);

        public Movimentosporconciliar ReservaCreditoExistenteNosMovimentos(int idReserva);
    }
}
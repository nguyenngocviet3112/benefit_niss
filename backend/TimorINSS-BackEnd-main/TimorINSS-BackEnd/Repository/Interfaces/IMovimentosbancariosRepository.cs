using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.ModelDataContract;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IMovimentosbancariosRepository : IDataRepository<Movimentosbancarios, MovimentosbancariosDto>
    {
        public Movimentosbancarios GetByTarefaAtivoId(long tarefaAtivoId);

        public MovimentosListagemResponse GetMovimentosByFilter(MovimentosListagemRequest request);

        public bool TemConciliados(List<int> items);

        public MovimentosListagemResponse GetListagemConciliacao(MovimentosBancarioConciliacaoListagemRequest request, int idEstadoPagamentoEmitido);

        public List<decimal> GetValores(List<int> ids);

        // Full entities (incl. conta_fk) cho 1 danh sách Id — dùng khi cần tra
        // "dòng sao kê này thuộc tài khoản ngân hàng nào" (vd để tự sinh bút
        // toán Débito đúng ngân hàng thật đã nhận tiền).
        public List<Movimentosbancarios> GetByIds(List<int> ids);

        public decimal? GetCreditoConciliados(int? contaId, int? caixaId);

        public decimal? GetDebitoConciliados(int? contaId, int? caixaId);

        public decimal? GetCreditoPorConciliar(int? contaId, int? caixaId);

        public decimal? GetDebitoPorConciliar(int? contaId, int? caixaId);

        public string ListMovimentosExcel(MovimentosListagemRequest request, List<MovimentosData> lista);
    }
}
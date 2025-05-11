using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IGuiaPagamentoDataManager
    {
        public GuiapagamentoDto GetDto(int id);

        public ResponseBaseDataContract SaveGuiaPagamento(GuiaPagamentoRequest trabalhador);

        public GuiaListagemResponse listGuiasByEntidade(GetAllGuiasStatesFromYearByFilterRequest request);

        public GuiaListagemResponse listGuiasByEntidadeApprove(GetAllGuiasStatesFromDateByFilterRequest request);


        public ResponseBaseDataContract useCreditInGuiaPagamento(UseCreditInGuiaPagamentoRequest request);

        public ResponseBaseDataContract insertComprovativoPagamento(insertComprovativoPagamentoRequest request);

        public ResponseBaseDataContract approveComprovativoPagamento(insertComprovativoPagamentoRequest request);

        public RelatorioGuiaPagamentoListagemResponse GetGuiasPagamentoRelatorios(RelatorioGuiaPagamentoListagemRequest request);
    }
}
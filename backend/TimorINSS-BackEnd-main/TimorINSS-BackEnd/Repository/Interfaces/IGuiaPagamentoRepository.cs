using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DTO;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Repository.Interfaces
{
    public interface IGuiaPagamentoRepository : IDataRepository<Guiapagamento, GuiapagamentoDto>
    {
        public string GetNextNumDocumento();

        public GuiaListagemResponse getGuiasByFilter(GetAllGuiasStatesFromYearByFilterRequest request);

        public GuiaListagemResponse getGuiasAporoveByFilter(GetAllGuiasStatesFromDateByFilterRequest request);

        public decimal SumValorPago(int idContaCorrente);

        public List<Guiapagamento> GetAllGuiasAtrasadas(long geradaStateId);

        public List<Guiapagamento> GetGuiasExpiradasPorMesAno(int mes, int ano);

        //public Guiapagamento GetUltimoGuiaFilho(int? idGuiaPai);

        public List<Guiapagamento> GetGuiasByIds(List<int> idGuias);

        public RelatorioGuiaPagamentoListagemResponse GetGuiasPagamentoRelatorios(RelatorioGuiaPagamentoListagemRequest request);
    }
}
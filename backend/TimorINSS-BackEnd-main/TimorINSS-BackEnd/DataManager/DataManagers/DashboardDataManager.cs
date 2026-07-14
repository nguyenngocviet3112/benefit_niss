using System;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class DashboardDataManager : IDashboardDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DashboardSummaryResponse GetSummary()
        {
            DashboardSummaryResponse response = new DashboardSummaryResponse();
            try
            {
                var summary = _unitOfWork.DashboardRepository.GetSummary();

                // Thực chi/Ngân sách + Guia Pagamento đã validate/tổng — năm hiện tại, kết hợp từ 2
                // repository khác (theo đúng pattern DataManager tổng hợp qua IUnitOfWork, không cho
                // 1 Repository gọi thẳng Repository khác).
                var year = DateTime.Now.Year;
                var totaisDespesa = _unitOfWork.CeInssGlobalRepository.GetTotaisDespesa(year, null);
                summary.OrcamentoTotal = totaisDespesa.Orcamento;
                summary.DespesaExecutada = totaisDespesa.Executado;

                var validacaoGp = _unitOfWork.GuiaPagamentoRepository.GetValidacaoSummary(year);
                summary.GuiaPagamentoValidado = validacaoGp.Validado;
                summary.GuiaPagamentoTotal = validacaoGp.Total;

                response.Summary = summary;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return response;
        }
    }
}

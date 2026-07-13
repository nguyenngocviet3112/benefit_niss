using System;
using System.Collections.Generic;
using System.Linq;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Extensions;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class CeInssGlobalDataManager : ICeInssGlobalDataManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsDataManager _utils;

        public CeInssGlobalDataManager(IUnitOfWork unitOfWork, IUtilsDataManager utils)
        {
            _unitOfWork = unitOfWork;
            _utils = utils;
        }

        public CeInssGlobalResponse GetReport(CeInssGlobalRequest request)
        {
            var response = new CeInssGlobalResponse { RequestId = request.RequestId };

            bool permission = _utils.ValidatePermission(request.UserId, (int)ModuleRelatorios.Relatorios, _unitOfWork);
            if (!permission)
            {
                response.Errors.Add(new Error { ErrorCode = ((int)ErrorsDataContract.InvalidPermission).ToString(), ErrorMessage = ErrorsDataContract.InvalidPermission.ToString() });
                return response;
            }

            try
            {
                response = _unitOfWork.CeInssGlobalRepository.GetReport(request);
                response.RequestId = request.RequestId;
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }

        public StringFileReponse GetReportExcel(CeInssGlobalRequest request)
        {
            var response = new StringFileReponse { RequestId = request.RequestId };

            var report = GetReport(request);
            if (report.Errors.Count > 0)
            {
                response.Errors = report.Errors;
                return response;
            }

            try
            {
                var headers = new[] { "Código", "Designação", "Orçamento Inicial", "Orçamentado",
                    "Cabimentos", "Compromissos", "Total Execução", "Taxa Execução (%)", "Saldo Execução",
                    "Comprometido Não Liquidado", "Cabimentado Não Comprometido" };

                var rows = new List<object[]>();
                rows.Add(new object[] { "RECEITAS", "", null, null, null, null, null, null, null, null, null });
                foreach (var r in report.receitas)
                {
                    rows.Add(new object[] { r.codigo, r.designacao, r.valorOrcamentoInicial, r.valorOrcamentado,
                        r.cabimentos, r.compromissos, r.totalExecucao, r.taxaExecucao, r.saldoExecucao,
                        r.saldoComprometidoNaoLiquidado, r.saldoCabimentadoNaoComprometido });
                }
                rows.Add(new object[] { "DESPESAS", "", null, null, null, null, null, null, null, null, null });
                foreach (var d in report.despesas)
                {
                    rows.Add(new object[] { d.codigo, d.designacao, d.valorOrcamentoInicial, d.valorOrcamentado,
                        d.cabimentos, d.compromissos, d.totalExecucao, d.taxaExecucao, d.saldoExecucao,
                        d.saldoComprometidoNaoLiquidado, d.saldoCabimentadoNaoComprometido });
                }

                response.File = ExcelExportHelper.BuildXlsxBase64("CE_OSS_Global", headers, rows);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }
    }
}

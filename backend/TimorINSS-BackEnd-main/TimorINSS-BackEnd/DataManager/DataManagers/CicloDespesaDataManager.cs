using System;
using System.Collections.Generic;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Extensions;
using TimorINSSBackEnd.Repository.Interfaces;

namespace TimorINSSBackEnd.DataManager.DataManagers
{
    public class CicloDespesaDataManager : ICicloDespesaDataManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CicloDespesaDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CicloDespesaListResponse GetByAno(GetCicloDespesaListRequest request)
        {
            var response = new CicloDespesaListResponse { RequestId = request.RequestId };

            try
            {
                response.items = _unitOfWork.CicloDespesaRepository.GetByAno(request.Ano, request.Institution);
            }
            catch (Exception e)
            {
                response.Errors.Add(new DataContracts.ResponseDataContract.Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }

        public StringFileReponse GetByAnoExcel(GetCicloDespesaListRequest request)
        {
            var response = new StringFileReponse { RequestId = request.RequestId };

            try
            {
                var items = _unitOfWork.CicloDespesaRepository.GetByAno(request.Ano, request.Institution);

                var headers = new[] { "Regime", "Atividade", "Classificação Económica", "Classificação Funcional",
                    "N.º AD", "Cabimentos", "Compromissos", "Saldo 1", "Obrigações", "Saldo 2", "Pagamentos", "Saldo 3" };

                var rows = new List<object[]>();
                foreach (var i in items)
                {
                    rows.Add(new object[] {
                        $"{i.regimeCodigo} {i.regimeDesignacao}", $"{i.atividadeCodigo} {i.atividadeDesignacao}",
                        $"{i.classificacaoEconomicaCodigo} {i.classificacaoEconomicaDesignacao}",
                        i.classificacaoFuncionalCodigo != null ? $"{i.classificacaoFuncionalCodigo} {i.classificacaoFuncionalDesignacao}" : "",
                        i.numeroAd, i.cabimentos, i.compromissos, i.saldo1, i.obrigacoes, i.saldo2, i.pagamentos, i.saldo3
                    });
                }

                response.File = ExcelExportHelper.BuildXlsxBase64("Ciclo da Despesa", headers, rows);
            }
            catch (Exception e)
            {
                response.Errors.Add(new DataContracts.ResponseDataContract.Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            return response;
        }
    }
}

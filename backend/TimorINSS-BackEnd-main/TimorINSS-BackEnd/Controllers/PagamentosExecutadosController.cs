using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/pagamentoExecutado")]
    [ApiController]
    public class PagamentosExecutadosController : ControllerBase
    {
        private readonly IPagamentoExecutadoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public PagamentosExecutadosController(IPagamentoExecutadoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("SavePagamentoExecutado")]
        public IActionResult SavePagamentoExecutado(SavePagamentoExecutadoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = request.importId.HasValue ? _dataManager.SavePagamentoExecutadoImport(request) : _dataManager.SavePagamentoExecutado(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SavePagamentoExecutado", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDestinatariosPagamentoByNumPagamento")]
        public IActionResult GetDestinatariosPagamentoByNumPagamento(GetDestinatarioPagamentoRequest request)
        {
            DestinatarioPagamentoExecutadosResponse response = new DestinatarioPagamentoExecutadosResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDestinatariosPagamentoByNumPagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDestinatariosPagamentoByNumPagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeletePagamento")]
        public IActionResult DeletePagamento(DeletePagamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeletePagamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeletePagamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetPagamentosExecutadosByIdDestinatario")]
        public IActionResult GetPagamentosExecutadosByIdDestinatario(GetPagamentoExecutadoRequest request)
        {
            DestinatarioPagamentoExecutadosResponse response = new DestinatarioPagamentoExecutadosResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetPagamentosExecutadosByIdDestinatario(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPagamentosExecutadosByIdDestinatario", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetListaPagamentoExcel")]
        public IActionResult GetListaPagamentoExcel(ListagemPagamentosProcessoRequest request)
        {
            DestinatarioPagamentoExecutadosResponse response = new DestinatarioPagamentoExecutadosResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetListaPagamentoExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetListaPagamentoExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditPagamentoExecutado")]
        public IActionResult EditPagamentoExecutado(EditPagamentoExecutadoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditPagamentoExecutado(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditPagamentoExecutado", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetPagamentosRelatoriosGrouped")]
        public IActionResult GetPagamentosRelatoriosGrouped(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetPagamentosRelatoriosGrouped(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPagamentosRelatoriosGrouped", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDropdownContasOGE")]
        public IActionResult GetDropdownContasOGE(RelatorioContaOGEDropdownListagemRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDropdownContasOGE(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDropdownContasOGE", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ExtractToExcelRelatorios")]
        public IActionResult ExtractToExcelRelatorios(RelatorioPagamentosListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ExtractToExcelRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ExtractToExcelRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetPagamentosRelatorios")]
        public IActionResult GetPagamentosRelatorios(RelatorioPagamentosListagemRequest request)
        {
            RelatorioPagamentosListagemResponse response = new RelatorioPagamentosListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetPagamentosRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPagamentosRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetCentrosCusto")]
        public IActionResult GetCentrosCusto(SearchFilterRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();
            try
            {
                response = _dataManager.GetCentrosCusto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPagamentosRelatorios", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetExecucaoOrcamental")]
        public IActionResult GetExecucaoOrcamental(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            ExecucaoOrcamentalListagemResponse response = new ExecucaoOrcamentalListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetExecucaoOrcamental(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetExecucaoOrcamental", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetExecucaoOrcamentalExcel")]
        public IActionResult GetExecucaoOrcamentalExcel(RelatorioExecucaoOrcamentalListagemRequest request)
        {
            StringFileReponse response = new StringFileReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetExecucaoOrcamentalExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetExecucaoOrcamentalExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("GetListaPagamentosProcesso")]
        public IActionResult GetListaPagamentosProcesso(ListagemPagamentosProcessoRequest request)
        {
            ListagemPagamentosProcessoResponse response = new ListagemPagamentosProcessoResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetListaPagamentosProcesso(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetListaPagamentosProcesso", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetPagamentoDetails")]
        public IActionResult GetPagamentoDetails(GetDestinatarioPagamentoRequest request)
        {
            ListagemPagamentosProcessoResponse response = new ListagemPagamentosProcessoResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetPagamentoDetails(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPagamentoDetails", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ClassificacaoContabilisticaRelatorios")]
        public IActionResult ClassificacaoContabilisticaRelatorios(SearchFilterRequest request)
        {
            ClassificacaoContabilisticaRelatorios response = new ClassificacaoContabilisticaRelatorios();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ClassificacaoContabilisticaRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ClassificacaoContabilisticaRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("ClassificacaoContabilisticaRelatoriosExcel")]
        public IActionResult ClassificacaoContabilisticaRelatoriosExcel(FornecedoresRelatoriosRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ClassificacaoContabilisticaRelatoriosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ClassificacaoContabilisticaRelatoriosExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("FornecedoresRelatorios")]
        public IActionResult FornecedoresRelatorios(FornecedoresRelatoriosRequest request)
        {
            FornecedoresRelatorios response = new FornecedoresRelatorios();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.FornecedoresRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("FornecedoresRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("FornecedoresRelatoriosExcel")]
        public IActionResult FornecedoresRelatoriosExcel(FornecedoresRelatoriosRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.FornecedoresRelatoriosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("FornecedoresRelatoriosExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPost("BalancoRelatorios")]
        public IActionResult BalancoRelatorios(BalancoRelatoriosRequest request)
        {
            BalancoRelatorios response = new BalancoRelatorios();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.BalancoRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("BalancoRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("BalancoRelatoriosExcel")]
        public IActionResult BalancoRelatoriosExcel(BalancoRelatoriosRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.BalancoRelatoriosExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("BalancoRelatoriosExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SaveClassificacaoExecucao")]
        public IActionResult SaveClassificacaoExecucao(SaveClassificacaoContabilisticaExecucaoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveClassificacaoExecucao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveClassificacaoExecucao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }



}
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/processos")]
    [ApiController]
    public class ProcessosController : ControllerBase
    {
        private readonly IProcessosDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProcessosController(IProcessosDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetAllProcessos")]
        public IActionResult GetAllProcessos(SearchFilterRequest request)
        {
            ProcessosListagemResponse response = new ProcessosListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllProcessos(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllProcessos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SwitchProcessoState")]
        public IActionResult SwitchProcessoState(SwitchProcessoStateRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SwitchProcessoState(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SwitchProcessoState", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("CreateProcessoConfig")]
        public IActionResult CreateProcessoConfig(ProcessoConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CreateProcessoConfig(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("CreateProcessoConfig", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetProcessoConfig")]
        public IActionResult GetProcessoConfig(ListProcessoConfiRequest request)
        {
            ProcessoConfigListagemResponse response = new ProcessoConfigListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetProcessoConfig(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetProcessoConfig", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateProcessoConfig")]
        public IActionResult UpdateProcessoConfig(ProcessoConfigRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateProcessoConfig(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateProcessoConfig", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListIniciarProcessos")]
        public IActionResult ListIniciarProcessos(RequestBaseDataContract request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListIniciarProcessos(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListIniciarProcessos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListIniciarProcessosApprove")]
        public IActionResult ListIniciarProcessosApprove(RequestBaseDataContract request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ListIniciarProcessApprove(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListIniciarProcessos", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllProcessosArquivados")]
        public IActionResult GetAllProcessosArquivados(SearchFilterRequest request)
        {
            ProcessosArquivadosListagemResponse response = new ProcessosArquivadosListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllProcessosArquivados(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllProcessosArquivados", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetProcessosRelatorios")]
        public IActionResult GetProcessosRelatorios(RelatorioProcessosListagemRequest request)
        {
            RelatorioProcessosListagemResponse response = new RelatorioProcessosListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetProcessosRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetProcessosRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetProcessoData")]
        public IActionResult GetProcessoData(GetProcessoDataRequest request)
        {
            ProcessoDataResponse response = new ProcessoDataResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetProcessoData(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetProcessoData", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("GetHistoricoTexto")]
        public IActionResult GetHistoricoTexto(GetHistoricoTextoProcessoRequest request)
        {
            ComponenteHistoricoTextoListagemResponse response = new ComponenteHistoricoTextoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetHistoricoTexto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetHistoricoTexto", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("StartProcess")]
        public IActionResult StartProcess(StartProcessRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.StartProcess(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("StartProcess", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetTipoProcessosRelatorios")]
        public IActionResult GetTipoProcessosRelatorios(RequestBaseDataContract request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTipoProcessosRelatorios(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetTipoProcessosRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ExtractToExcelRelatorios")]
        public IActionResult ExtractToExcelRelatorios(RelatorioProcessosListagemRequest request)
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
    }
}
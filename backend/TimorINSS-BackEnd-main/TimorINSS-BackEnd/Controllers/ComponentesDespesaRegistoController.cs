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
    [Route("api/componenteDespesaRegisto")]
    [ApiController]
    public class ComponentesDespesaRegistoController : ControllerBase
    {
        private readonly IComponenteDespesaRegistoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesDespesaRegistoController(IComponenteDespesaRegistoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("AddEditComponenteDespesaRegisto")]
        public IActionResult AddEditComponenteDespesaRegisto(RegistoDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddEditComponenteDespesaRegisto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddEditComponenteDespesaRegisto", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllDespesaRegistadaByTarefaAtivoId")]
        public IActionResult GetAllDespesaRegistadaByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaRegistoReponse response = new GetComponenteDespesaRegistoReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllDespesaRegistadaByTarefaAtivoId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllDespesaRegistadaByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

       [HttpPost("DeleteDespesa")]
        public IActionResult DeleteDespesa(DeleteDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteDespesa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteDespesa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetValoresDespesaByIdCodigoOrcamento")]
        public IActionResult GetValoresDespesaByIdCodigoOrcamento(GetValoresDespesaByIdCodigoOrcamentoRequest request)
        {
            GetValoresDespesaByIdCodigoOrcamentoResponse response = new GetValoresDespesaByIdCodigoOrcamentoResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetValoresDespesaByIdCodigoOrcamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetValoresDespesaByIdCodigoOrcamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteAllDespesasRegistadas")]
        public IActionResult DeleteAllDespesasRegistadas(DeleteListaDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteAllDespesasRegistadas(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteAllDespesasRegistadas", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateDespesa")]
        public IActionResult UpdateDespesa(UpdateDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateDespesa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateDespesa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId")]
        public IActionResult GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaCabimentadaParaExecucaoReponse response = new GetComponenteDespesaCabimentadaParaExecucaoReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllDespesaCabimentadasParaExecucaoByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDespesasRelatorios")]
        public IActionResult GetDespesasRelatorios(GetDespesasRelatorioRequest request)
        {
            GetDespesasRelatoriosReponse response = new GetDespesasRelatoriosReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDespesasRelatorio(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDespesasRelatorios", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDespesasRelatoriosExcel")]
        public IActionResult GetDespesasRelatoriosExcel(GetDespesasRelatorioRequest request)
        {
            StringFileReponse response = new StringFileReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDespesasRelatorioExcel(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDespesasRelatoriosExcel", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetDespesasCompromissoByTarefaAtivoId")]
        public IActionResult GetDespesasCompromissoByTarefaAtivoId(GetDespesasCompromissoRequest request)
        {
            GetDespesasCompromissoResponse response = new GetDespesasCompromissoResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetDespesasCompromissoByTarefaAtivoId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDespesasObrigacaoByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpdateDespesaCabimentada")]
        public IActionResult UpdateDespesaCabimentada(UpdateDespesaCabimentadaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateDespesaCabimentada(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateDespesa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteCompromisso")]
        public IActionResult DeleteCompromisso(DeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteCompromisso(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDespesasObrigacaoByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UpsertCompromisso")]
        public IActionResult UpsertCompromisso(CompromissoUpsertRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpsertCompromisso(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDespesasObrigacaoByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId")]
        public IActionResult GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(GetAllDespesaRegistadaRequest request)
        {
            GetComponenteDespesaCabimentadaParaExecucaoReponse response = new GetComponenteDespesaCabimentadaParaExecucaoReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllDespesaCompromissosParaExecucaoByTarefaAtivoId", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
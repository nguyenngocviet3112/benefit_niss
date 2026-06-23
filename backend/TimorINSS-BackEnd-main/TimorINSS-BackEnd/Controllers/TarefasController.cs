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
    [Route("api/tarefa")]
    [ApiController]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public TarefasController(ITarefaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetAllTarefas")]
        public IActionResult GetAllTarefas(SearchFilterRequest request)
        {
            TarefaListagemResponse response = new TarefaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllTarefas(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTarefas", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllTarefaAtivo")]
        public IActionResult GetAllTarefaAtivo()
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                response = _dataManager.GetAllTarefaAtivo();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTarefaAtivo", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("AddTarefaConfigurada")]
        public IActionResult AddTarefaConfigurada(ConfigurarTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddTarefaConfigurada(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddTarefaConfigurada", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllComponentesByIdTarefa/{id}")]
        public IActionResult GetAllComponentesByIdTarefa(int id)
        {
            ComponenteTarefaConfiguradaRespose response = new ComponenteTarefaConfiguradaRespose();
            RequestBaseDataContract request = new RequestBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllComponentesByIdTarefa(id, request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllComponentesByIdTarefa", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarTarefa")]
        public IActionResult EditarTarefa(TarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllTarefasAtivas")]
        public IActionResult GetAllTarefasAtivas(SearchFilterRequest request)
        {
            TarefasAtivasListagemResponse response = new TarefasAtivasListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllTarefasAtivas(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTarefasAtivas", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("LockTarefa")]
        public IActionResult LockTarefa(SwitchTarefaAtivoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.LockTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("LockTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("UnlockTarefa")]
        public IActionResult UnlockTarefa(SwitchTarefaAtivoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UnlockTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UnlockTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetHistoricoTexto")]
        public IActionResult GetHistoricoTexto(GetHistoricoTextoRequest request)
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

        [HttpPost("GetAllTarefasASeguir")]
        public IActionResult GetAllTarefasASeguir(GetAllTarefasASeguirRequest request)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllTarefasASeguir(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllTarefasASeguir", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("GetTarefaData")]
        public IActionResult GetTarefaData(GetTarefaDataRequest request)
        {
            TarefaDataResponse response = new TarefaDataResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTarefaData(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetTarefaData", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("SaveTarefaData")]
        public IActionResult SaveTarefaData(SaveTarefaDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveTarefaData(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveTarefaData", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("ArquivarTarefa")]
        public IActionResult ArquivarTarefa(SaveTarefaDataRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ArquivarTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ArquivarTarefa", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
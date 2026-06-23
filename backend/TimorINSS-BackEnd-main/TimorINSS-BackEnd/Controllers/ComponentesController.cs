using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/componente")]
    [ApiController]
    public class ComponentesController : ControllerBase
    {
        private readonly IComponenteDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesController(IComponenteDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllComponentes")]
        public IActionResult GetAllComponentes()
        {
            ComponentesListagemResponse response = new ComponentesListagemResponse();

            try
            {
                response = _dataManager.GetAllComponentes();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllComponentes", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteTexto")]
        public IActionResult EditarComponenteTexto(ComponenteTextoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteTexto(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteTexto", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponentePrazoTarefa")]
        public IActionResult EditarComponentePrazoTarefa(ComponentePrazoTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponentePrazoTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponentePrazoTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteAccaoTarefa")]
        public IActionResult EditarComponenteAccaoTarefa(ComponenteAccaoTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteAccaoTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteAccaoTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteDocumentoTarefa")]
        public IActionResult EditarComponenteDocumentoTarefa(ComponenteDocumentoTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteDocumentoTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteDocumentoTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteClassificacaoSubClassifTarefa")]
        public IActionResult EditarComponenteClassificacaoSubClassifTarefa(ComponenteClassificacaoSubClassificTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteClassificacaoSubClassifTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteClassificacaoSubClassifTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteControloAcessoPerfilTarefa")]
        public IActionResult EditarComponenteControloAcessoPerfilTarefa(ComponenteControloAcessoPerfilTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteControloAcessoPerfilTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteControloAcessoPerfilTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteControloAcessoUtilizadorTarefa")]
        public IActionResult EditarComponenteControloAcessoUtilizadorTarefa(ComponenteControloAcessoUtilizadorTarefaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteControloAcessoUtilizadorTarefa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteControloAcessoUtilizadorTarefa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteOrcamento")]
        public IActionResult EditarComponenteOrcamento(ComponenteOrcamentoRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteOrcamento(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteOrcamento", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteDespesa")]
        public IActionResult EditarComponenteDespesa(ComponenteDespesaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteDespesa(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteDespesa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteConciliacaoMovimentos")]
        public IActionResult EditarComponenteConciliacaoMovimentos(ComponenteConciliacaoMovimentosRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteConciliacaoMovimentos(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteDespesa", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditarComponenteReceita")]
        public IActionResult EditarComponenteReceita(ComponenteReceitaRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditarComponenteReceita(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditarComponenteReceita", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
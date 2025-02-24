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
    [Route("api/componenteOrcamentoValor")]
    [ApiController]
    public class ComponentesOrcamentoValorController : ControllerBase
    {
        private readonly IComponenteOrcamentoValorDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesOrcamentoValorController(IComponenteOrcamentoValorDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("AddOrcamentoValor")]
        public IActionResult AddOrcamentoValor([FromBody] AddComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddOrcamentoValor(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddOrcamentoValor", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SearchOrcamentoValor")]
        public IActionResult SearchOrcamentoValor([FromBody] SearchComponenteOrcamentoValorRequest request)
        {
            SearchComponentesOrcamentoValorResponse response = new SearchComponentesOrcamentoValorResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SearchOrcamentoValor(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddOrcamentoValor", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EditOrcamentoValor")]
        public IActionResult EditOrcamentoValor([FromBody] AddComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditOrcamentoValor(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EditOrcamentoValor", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("EliminarOrcamentoValor")]
        public IActionResult EliminarOrcamentoValor([FromBody] EliminarComponenteOrcamentoValorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EliminarOrcamentoValor(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("EliminarOrcamentoValor", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
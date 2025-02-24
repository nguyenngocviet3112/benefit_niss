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
    [Route("api/relTarefaComponente")]
    [ApiController]
    public class RelTarefasComponentesController : ControllerBase
    {
        private readonly IRelTarefaComponenteDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public RelTarefasComponentesController(IRelTarefaComponenteDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("UpdateRelTarefaComponente")]
        public IActionResult UpdateRelTarefaComponente(UpdateRelTarefaComponenteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateRelTarefaComponente(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateRelTarefaComponente", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllRelTarefaComponenteByIdTarefaActivo")]
        public IActionResult GetAllRelTarefaComponenteByIdTarefaActivo(GetAllRelTarefaComponenteByIdTarefaActivoRequest request)
        {
            ComponentesListagemResponse response = new ComponentesListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllRelTarefaComponenteByIdTarefaActivo(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllRelTarefaComponenteByIdTarefaActivo", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
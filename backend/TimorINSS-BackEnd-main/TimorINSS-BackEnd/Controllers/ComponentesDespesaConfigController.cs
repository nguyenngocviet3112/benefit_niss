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
    [Route("api/componenteDespesaConfig")]
    [ApiController]
    public class ComponentesDespesaConfigController : ControllerBase
    {
        private readonly IComponenteDespesaConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ComponentesDespesaConfigController(IComponenteDespesaConfigDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetComponenteDespesaConfigByTarefaAtivoId")]
        public IActionResult GetComponenteDespesaConfigByTarefaAtivoId(GetComponenteDespesaConfigRequest request)
        {
            GetComponenteDespesaConfigReponse response = new GetComponenteDespesaConfigReponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetComponenteDespesaConfigByTarefaAtivoId(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetComponenteDespesaConfigByTarefaAtivoId", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
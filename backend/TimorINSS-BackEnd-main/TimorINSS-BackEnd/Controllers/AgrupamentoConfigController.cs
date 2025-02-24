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
    [Route("api/agrupamentoConfig")]
    [ApiController]
    public class AgrupamentoConfigController : ControllerBase
    {
        private readonly IAgrupamentoConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public AgrupamentoConfigController(IAgrupamentoConfigDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpPost("GetAgrupamentoConfigByIdCodigoContaTipoConta")]
        public IActionResult GetAgrupamentoConfigByIdCodigoContaTipoConta([FromBody] GetAgrupamentoConfigRequest request)
        {
            AgrupamentoConfigReponse response = new AgrupamentoConfigReponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAgrupamentoConfigByIdCodigoContaTipoConta(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAgrupamentoConfigByIdCodigoContaTipoConta", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
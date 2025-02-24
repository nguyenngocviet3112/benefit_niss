using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/escaloes")]
    [ApiController]
    public class EscaloesController : ControllerBase
    {
        private readonly IEscalaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public EscaloesController(IEscalaoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllEscaloes")]
        public IActionResult GetAllEscaloes()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllEscaloes");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllEscaloes();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllEscaloes", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllEscaloes", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
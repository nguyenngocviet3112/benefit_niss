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
    [Route("api/actividadeEconomica")]
    [ApiController]
    public class ActividadeEconomicaController : ControllerBase
    {
        private readonly IActividadeEconomicaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ActividadeEconomicaController(IActividadeEconomicaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getActividadeEconomica")]
        public IActionResult getActividadeEconomica()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getActividadeEconomica");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllActividadeEconomica();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getActividadeEconomica", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getActividadeEconomica", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
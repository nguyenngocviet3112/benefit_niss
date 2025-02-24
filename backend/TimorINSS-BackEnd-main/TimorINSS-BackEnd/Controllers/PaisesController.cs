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
    [Route("api/pais")]
    [ApiController]
    public class PaisesController : ControllerBase
    {
        private readonly IPaisDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public PaisesController(IPaisDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getPais")]
        public IActionResult getPais()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getPais");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getPais();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getPais", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getPais", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
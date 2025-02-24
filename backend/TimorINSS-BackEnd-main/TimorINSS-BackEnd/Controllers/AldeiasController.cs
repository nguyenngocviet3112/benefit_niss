using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/aldeia")]
    [ApiController]
    public class AldeiaController : ControllerBase
    {
        private readonly IAldeiaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public AldeiaController(IAldeiaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getAldeia")]
        public IActionResult getAldeia()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getAldeia");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllAldeia();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }
                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getAldeia", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getAldeia", response, response.selects.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("getAldeiaByIdSuco/{id}")]
        public IActionResult getAldeiaByIdSuco(int id)
        {
            SelectDescriptionResponse response = _dataManager.getAldeiaByIdSuco(id);

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getAldeiaByIdSuco", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
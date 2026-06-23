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
    [Route("api/suco")]
    [ApiController]
    public class SucosController : ControllerBase
    {
        private readonly ISucoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public SucosController(ISucoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getSuco")]
        public IActionResult getSuco()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getSuco");
            if (response == null)
            {
                response = _dataManager.getAllSuco();

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getSuco", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getSuco", response, response.selects.Count);
                }
            }
            return Ok(response);
        }

        [HttpGet("getSucoByIdPosto/{id}")]
        public IActionResult getSucoByIdPosto(int id)
        {
            SelectDescriptionResponse response = new SelectDescriptionResponse();

            try
            {
                response = _dataManager.getSucoByIdPosto(id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getSucoByIdPosto/{id}", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
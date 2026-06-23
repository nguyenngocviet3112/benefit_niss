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
    [Route("api/naturezaJuridica")]
    [ApiController]
    public class NaturezaJuridicaController : ControllerBase
    {
        private readonly INaturezaJuridicaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public NaturezaJuridicaController(INaturezaJuridicaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getNaturezaJuridica")]
        public IActionResult getNaturezaJuridica()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getNaturezaJuridica");
            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllNaturezaJuridica();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getNaturezaJuridica", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getNaturezaJuridica", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
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
    [Route("api/municipio")]
    [ApiController]
    public class MunicipiosController : ControllerBase
    {
        private readonly IMunicipioDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public MunicipiosController(IMunicipioDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getMunicipio")]
        public IActionResult getMunicipio()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getMunicipio");
            if (response == null)
            {
                try
                {
                    response = _dataManager.getAllMunicipio();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getMunicipio", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getMunicipio", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
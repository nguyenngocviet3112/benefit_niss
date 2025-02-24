using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/sectorActividade")]
    [ApiController]
    public class SectorActividadeController : ControllerBase
    {
        private readonly ISectorActividadeDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public SectorActividadeController(ISectorActividadeDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("getSectorActividade")]
        public IActionResult getSectorActividade()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("getSectorActividade");
            if (response == null)
            {
                response = _dataManager.GetAllSectorActividade();

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("getSectorActividade", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("getSectorActividade", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
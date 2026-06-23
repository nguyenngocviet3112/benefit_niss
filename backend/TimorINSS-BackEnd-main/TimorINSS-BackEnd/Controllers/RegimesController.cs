using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/regimes")]
    [ApiController]
    public class RegimesController : ControllerBase
    {
        private readonly IRegimeDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public RegimesController(IRegimeDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }
    }
}
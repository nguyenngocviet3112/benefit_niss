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
    [Route("api/institution")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public InstitutionController(IInstitutionDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllInstitutionsAtivo")]
        public IActionResult GetAllInstitutionAtivo()
        {
            SelectDescriptionResponse response = _cache.GetFromCache<SelectDescriptionResponse>("GetAllInstitutionsAtivo");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllInstitutionAtivo();
                }
                catch (Exception e)
                {
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                // Guardar log do erro no ficheiro de logs
                if (response.ManageErrors("GetAllInstitutionsAtivo", Log, null))
                {
                    return BadRequest(response);
                }
                else
                {
                    _cache.SetCache("GetAllInstitutionsAtivo", response, response.selects.Count);
                }
            }
            return Ok(response);
        }
    }
}
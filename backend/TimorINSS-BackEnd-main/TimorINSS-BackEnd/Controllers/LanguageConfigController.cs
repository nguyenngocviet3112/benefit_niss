using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/languageconfig")]
    [ApiController]
    public class LanguageConfigController : ControllerBase
    {
        private readonly ILanguageConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public LanguageConfigController(ILanguageConfigDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        // Admin screen — lists all languages, including disabled ones.
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            LanguageConfigListResponse response;
            try
            {
                response = _dataManager.GetAll();
            }
            catch (Exception e)
            {
                response = new LanguageConfigListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAll", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        // Public — drives the language dropdown shown on the login screen
        // (before any token exists), so it cannot require [Authorize].
        [AllowAnonymous]
        [HttpGet("GetActive")]
        public IActionResult GetActive()
        {
            LanguageConfigListResponse response = _cache.GetFromCache<LanguageConfigListResponse>("LanguageConfigGetActive");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllActive();
                }
                catch (Exception e)
                {
                    response = new LanguageConfigListResponse();
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                if (response.ManageErrors("GetActive", Log, null))
                    return BadRequest(response);

                _cache.SetCache("LanguageConfigGetActive", response, response.Items.Count);
            }
            return Ok(response);
        }

        [HttpPost("Toggle")]
        public IActionResult Toggle(ToggleLanguageConfigRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Toggle(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Toggle", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}

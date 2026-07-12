using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/codigocontaopeningbalance")]
    [ApiController]
    public class CodigoContaOpeningBalanceController : ControllerBase
    {
        private readonly ICodigoContaOpeningBalanceDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public CodigoContaOpeningBalanceController(ICodigoContaOpeningBalanceDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            CodigoContaOpeningBalanceListResponse response;
            try
            {
                response = _dataManager.GetAll();
            }
            catch (Exception e)
            {
                response = new CodigoContaOpeningBalanceListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAll", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Update")]
        [RequirePerm("ABE_SUBMIT")]
        public IActionResult Update(UpdateCodigoContaOpeningBalanceRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Update(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Update", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}

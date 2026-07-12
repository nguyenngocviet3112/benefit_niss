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
    [Route("api/functionalclassification")]
    [ApiController]
    public class FunctionalClassificationController : ControllerBase
    {
        private readonly IFunctionalClassificationDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public FunctionalClassificationController(IFunctionalClassificationDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAllActive")]
        public IActionResult GetAllActive()
        {
            FunctionalClassificationTreeResponse response = _cache.GetFromCache<FunctionalClassificationTreeResponse>("GetAllActiveFunctionalClassification");

            if (response == null)
            {
                try
                {
                    response = _dataManager.GetAllActive();
                }
                catch (Exception e)
                {
                    response = new FunctionalClassificationTreeResponse();
                    response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
                }

                if (response.ManageErrors("GetAllActive", Log, null))
                    return BadRequest(response);

                _cache.SetCache("GetAllActiveFunctionalClassification", response, response.Items.Count);
            }
            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveFunctionalClassificationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveFunctionalClassification(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Deactivate")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Deactivate(DeactivateFunctionalClassificationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateFunctionalClassification(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Deactivate", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Import")]
        [RequestSizeLimit(20000000)]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Import([FromForm] ImportMasterDataTreeRequest request)
        {
            ImportMasterDataTreeResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ImportFunctionalClassification(request);
            }
            catch (Exception e)
            {
                response = new ImportMasterDataTreeResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Import", Log, null))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}

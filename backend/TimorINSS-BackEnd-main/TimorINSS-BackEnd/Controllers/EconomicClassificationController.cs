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
    [Route("api/economicclassification")]
    [ApiController]
    public class EconomicClassificationController : ControllerBase
    {
        private readonly IEconomicClassificationDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public EconomicClassificationController(IEconomicClassificationDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetTree/{orcamentoConfigFk}")]
        public IActionResult GetTree(int orcamentoConfigFk)
        {
            EconomicClassificationTreeResponse response;
            try
            {
                GetEconomicClassificationTreeRequest request = new GetEconomicClassificationTreeRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetTreeByOrcamentoConfig(request);
            }
            catch (Exception e)
            {
                response = new EconomicClassificationTreeResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetTree", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveEconomicClassificationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveEconomicClassification(request);
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
        public IActionResult Deactivate(DeactivateEconomicClassificationRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateEconomicClassification(request);
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
                response = _dataManager.ImportEconomicClassification(request);
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

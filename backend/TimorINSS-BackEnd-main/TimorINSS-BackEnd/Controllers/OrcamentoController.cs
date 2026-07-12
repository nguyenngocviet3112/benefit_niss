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
    [Route("api/orcamento")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public OrcamentoController(IOrcamentoDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetActiveBatch/{orcamentoConfigFk}")]
        public IActionResult GetActiveBatch(int orcamentoConfigFk)
        {
            OrcamentoBatchResponse response;
            try
            {
                GetActiveOrcamentoBatchRequest request = new GetActiveOrcamentoBatchRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetActiveBatch(request);
            }
            catch (Exception e)
            {
                response = new OrcamentoBatchResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetActiveBatch", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SaveLinha")]
        [RequirePerm("ORC_SUBMIT")]
        public IActionResult SaveLinha(SaveOrcamentoLinhaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveLinha(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SaveLinha", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("DeleteLinha")]
        [RequirePerm("ORC_SUBMIT")]
        public IActionResult DeleteLinha(DeleteOrcamentoLinhaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteLinha(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("DeleteLinha", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Submit")]
        [RequirePerm("ORC_SUBMIT")]
        public IActionResult Submit(SubmitOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Submit(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Submit", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Review")]
        [RequirePerm("ORC_REVIEW")]
        public IActionResult Review(ReviewOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Review(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Review", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Approve")]
        [RequirePerm("ORC_APPROVE")]
        public IActionResult Approve(ApproveOrcamentoBatchRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Approve(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Approve", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Import")]
        [RequestSizeLimit(20000000)]
        [RequirePerm("ORC_SUBMIT")]
        public IActionResult Import([FromForm] ImportOrcamentoRequest request)
        {
            ImportMasterDataTreeResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Import(request);
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

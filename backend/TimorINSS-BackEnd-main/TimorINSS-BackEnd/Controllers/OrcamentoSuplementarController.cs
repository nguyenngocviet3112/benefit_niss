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
    [Route("api/orcamentosuplementar")]
    [ApiController]
    public class OrcamentoSuplementarController : ControllerBase
    {
        private readonly IOrcamentoSuplementarDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public OrcamentoSuplementarController(IOrcamentoSuplementarDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetActiveBatch/{orcamentoConfigFk}")]
        public IActionResult GetActiveBatch(int orcamentoConfigFk)
        {
            OrcamentoSuplementarBatchResponse response;
            try
            {
                GetActiveOrcamentoSuplementarRequest request = new GetActiveOrcamentoSuplementarRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetActiveBatch(request);
            }
            catch (Exception e)
            {
                response = new OrcamentoSuplementarBatchResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetActiveBatch", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetRubricasAprovadas/{orcamentoConfigFk}")]
        public IActionResult GetRubricasAprovadas(int orcamentoConfigFk)
        {
            RubricasAprovadasParaSuplementarResponse response;
            try
            {
                GetRubricasAprovadasParaSuplementarRequest request = new GetRubricasAprovadasParaSuplementarRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetRubricasAprovadas(request);
            }
            catch (Exception e)
            {
                response = new RubricasAprovadasParaSuplementarResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetRubricasAprovadas", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SaveLinha")]
        public IActionResult SaveLinha(SaveOrcamentoSuplementarLinhaRequest request)
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
        public IActionResult DeleteLinha(DeleteOrcamentoSuplementarLinhaRequest request)
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
        public IActionResult Submit(SubmitOrcamentoSuplementarRequest request)
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
        public IActionResult Review(ReviewOrcamentoSuplementarRequest request)
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
        public IActionResult Approve(ApproveOrcamentoSuplementarRequest request)
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
    }
}

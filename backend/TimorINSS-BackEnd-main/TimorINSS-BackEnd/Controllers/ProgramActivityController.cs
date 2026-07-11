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
    [Route("api/programactivity")]
    [ApiController]
    public class ProgramActivityController : ControllerBase
    {
        private readonly IProgramActivityDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public ProgramActivityController(IProgramActivityDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetTree/{orcamentoConfigFk}")]
        public IActionResult GetTree(int orcamentoConfigFk)
        {
            ProgramActivityTreeResponse response;
            try
            {
                GetProgramActivityTreeRequest request = new GetProgramActivityTreeRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetTreeByOrcamentoConfig(request);
            }
            catch (Exception e)
            {
                response = new ProgramActivityTreeResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetTree", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        public IActionResult Save(SaveProgramActivityRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveProgramActivity(request);
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
        public IActionResult Deactivate(DeactivateProgramActivityRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateProgramActivity(request);
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

        [HttpPost("CopyYear")]
        public IActionResult CopyYear(CopyProgramActivityYearRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CopyProgramActivityYear(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("CopyYear", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}

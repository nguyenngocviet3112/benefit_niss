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
    // Manages OrcamentoConfig itself (kỳ/năm ngân sách — System Settings).
    // Deliberately separate from OrcamentoController (api/orcamento), which
    // manages OrcamentoBatch/OrcamentoLinha (rúbrica orçamental, M2) and is
    // not touched by this feature.
    [Authorize]
    [Route("api/orcamentoconfig")]
    [ApiController]
    public class OrcamentoConfigController : ControllerBase
    {
        private readonly IOrcamentoConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public OrcamentoConfigController(IOrcamentoConfigDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            OrcamentoConfigListResponse response;
            try
            {
                response = _dataManager.GetAll();
            }
            catch (Exception e)
            {
                response = new OrcamentoConfigListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAll", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        public IActionResult Save(SaveOrcamentoConfigRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Save(request);
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
        public IActionResult Deactivate(DeactivateOrcamentoConfigRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Deactivate(request);
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
    }
}

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
    [Route("api/codigocontatree")]
    [ApiController]
    public class CodigoContaTreeController : ControllerBase
    {
        private readonly ICodigoContaTreeDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public CodigoContaTreeController(ICodigoContaTreeDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetTree/{orcamentoConfigFk}")]
        public IActionResult GetTree(int orcamentoConfigFk)
        {
            CodigoContaTreeResponse response;
            try
            {
                GetCodigoContaTreeRequest request = new GetCodigoContaTreeRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                response = _dataManager.GetTreeByOrcamentoConfig(request);
            }
            catch (Exception e)
            {
                response = new CodigoContaTreeResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetTree", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveCodigoContaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveCodigoConta(request);
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
        public IActionResult Deactivate(DeactivateCodigoContaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateCodigoConta(request);
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

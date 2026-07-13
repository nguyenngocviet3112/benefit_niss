using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/liquidacaoContaConfig")]
    [ApiController]
    public class LiquidacaoContaConfigController : ControllerBase
    {
        private readonly ILiquidacaoContaConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LiquidacaoContaConfigController(ILiquidacaoContaConfigDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            LiquidacaoContaConfigListResponse response;
            try
            {
                response = _dataManager.GetAll();
            }
            catch (Exception e)
            {
                response = new LiquidacaoContaConfigListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetAll", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveLiquidacaoContaConfigRequest request)
        {
            LiquidacaoContaConfigListResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Save(request);
            }
            catch (Exception e)
            {
                response = new LiquidacaoContaConfigListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

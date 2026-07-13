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
    [Route("api/guiaPagamentoContaConfig")]
    [ApiController]
    public class GuiaPagamentoContaConfigController : ControllerBase
    {
        private readonly IGuiaPagamentoContaConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GuiaPagamentoContaConfigController(IGuiaPagamentoContaConfigDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetConfig")]
        public IActionResult GetConfig()
        {
            GuiaPagamentoContaConfigResponse response;
            try
            {
                response = _dataManager.GetConfig();
            }
            catch (Exception e)
            {
                response = new GuiaPagamentoContaConfigResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetConfig", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveGuiaPagamentoContaConfigRequest request)
        {
            GuiaPagamentoContaConfigResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveConfig(request);
            }
            catch (Exception e)
            {
                response = new GuiaPagamentoContaConfigResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

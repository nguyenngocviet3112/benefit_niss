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
    // Cấu hình tích hợp — bật/tắt cho phép các API gọi vào từ module khác
    // (hiện chỉ có Benefit module gọi vào api/benefit + api/benefit-data để
    // lấy thông tin NLĐ/công ty/lịch sử đóng góp). Tắt ở đây thì
    // RequireBenefitApiEnabledAttribute sẽ chặn (403) mọi request gọi tới
    // 2 controller đó, không cần sửa code phía Benefit.
    [Authorize]
    [Route("api/integrationconfig")]
    [ApiController]
    public class IntegrationConfigController : ControllerBase
    {
        private readonly IIntegrationConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IntegrationConfigController(IIntegrationConfigDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("Get")]
        public IActionResult Get()
        {
            IntegrationConfigResponse response;
            try
            {
                response = _dataManager.Get();
            }
            catch (Exception e)
            {
                response = new IntegrationConfigResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Get", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveIntegrationConfigRequest request)
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

            return Ok(response);
        }
    }
}

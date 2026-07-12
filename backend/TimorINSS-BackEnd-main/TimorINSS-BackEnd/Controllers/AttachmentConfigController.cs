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
    // Cấu hình Upload File — độ lớn tối đa + màn nào bắt buộc phải đính kèm file
    // trước khi submit (AD/Cabimento/Compromisso/Obrigação/Pagamento).
    [Authorize]
    [Route("api/attachmentconfig")]
    [ApiController]
    public class AttachmentConfigController : ControllerBase
    {
        private readonly IAttachmentConfigDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AttachmentConfigController(IAttachmentConfigDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // Không gắn RequirePerm — mọi user đăng nhập cần đọc được config này để
        // biết bước nào bắt buộc đính kèm file trước khi bấm Submit.
        [HttpGet("Get")]
        public IActionResult Get()
        {
            AttachmentConfigResponse response;
            try
            {
                response = _dataManager.Get();
            }
            catch (Exception e)
            {
                response = new AttachmentConfigResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Get", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveAttachmentConfigRequest request)
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

using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    // Đính kèm file (PDF/PNG/Excel) cho chu trình chi tiêu (AD/Cabimento/Compromisso/
    // Obrigação/Pagamento) — người submit chọn file khi submit, xem lại được sau đó.
    // Không gắn RequirePerm ở đây: bất kỳ user nào đã đủ quyền submit thực thể gốc
    // (đã chặn ở [RequirePerm] của chính hành động Submit) cũng cần đính/xem file được.
    [Authorize]
    [Route("api/attachment")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AttachmentController(IAttachmentDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetByEntity/{entityType}/{entityId}")]
        public IActionResult GetByEntity(string entityType, int entityId)
        {
            AttachmentListResponse response;
            try
            {
                GetAttachmentsByEntityRequest request = new GetAttachmentsByEntityRequest();
                request.GetHeaderInfo(Request.Headers);
                request.EntityType = entityType;
                request.EntityId = entityId;
                response = _dataManager.GetByEntity(request);
            }
            catch (Exception e)
            {
                response = new AttachmentListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByEntity", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Upload")]
        public IActionResult Upload(UploadAttachmentRequest request)
        {
            UploadAttachmentResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Upload(request);
            }
            catch (Exception e)
            {
                response = new UploadAttachmentResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Upload", Log, request))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("Download/{id}")]
        public IActionResult Download(int id)
        {
            var entity = _dataManager.GetForDownload(id);
            if (entity == null)
            {
                return NotFound();
            }
            // "inline" (not "attachment") so PDF/PNG open/preview in a new browser
            // tab instead of forcing a download dialog — Excel still downloads
            // since browsers have no native renderer for it either way.
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{entity.FileName}\"";
            return File(entity.FileContent, entity.ContentType);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(DeleteAttachmentRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.Delete(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Delete", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

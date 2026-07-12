using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    // Trang tổng quan mặc định khi đăng nhập mode mới. Gate bằng DASHBOARD_VIEW
    // (không phải ADMIN-only) — user chưa được gán quyền vẫn vào được route
    // (không phải trang lỗi), chỉ là gọi GetSummary sẽ trả 403 và frontend hiện
    // màn trống thay vì nội dung. Xem RequirePermAttribute.
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DashboardController(IDashboardDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetSummary")]
        [RequirePerm("DASHBOARD_VIEW")]
        public IActionResult GetSummary()
        {
            DashboardSummaryResponse response;
            try
            {
                response = _dataManager.GetSummary();
            }
            catch (Exception e)
            {
                response = new DashboardSummaryResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetSummary", Log, null))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

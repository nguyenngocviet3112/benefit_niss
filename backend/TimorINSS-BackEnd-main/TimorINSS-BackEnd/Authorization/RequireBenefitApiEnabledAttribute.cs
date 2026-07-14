using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Authorization
{
    // Cổng bật/tắt cho các endpoint mà Benefit module gọi vào
    // (BenefitController, BenefitDataController). Đọc IntegrationConfig từ
    // DB mỗi request — nếu admin tắt ở màn "Cấu hình tích hợp" thì trả 403
    // ngay tại đây, không cần đổi code phía Benefit hay tắt cả controller.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireBenefitApiEnabledAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dataManager = context.HttpContext.RequestServices.GetService(typeof(IIntegrationConfigDataManager)) as IIntegrationConfigDataManager;

            if (dataManager != null && !dataManager.IsBenefitApiEnabled())
            {
                context.Result = new ObjectResult(new
                {
                    errors = new[]
                    {
                        new { errorCode = "BENEFIT-API-DISABLED", errorMessage = "Tích hợp API Benefit hiện đang tắt trong Cấu hình tích hợp — liên hệ quản trị viên để bật lên." }
                    }
                })
                {
                    StatusCode = 403
                };
                return;
            }

            await next();
        }
    }
}

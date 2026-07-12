using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace TimorINSSBackEnd.Authorization
{
    // Granular RBAC gate for new-mode ("Módulo Contabilidade") business actions.
    // Checks the JWT "perms" claim (populated at login for internal-staff
    // accounts, see UtilizadorDataManager.GenerateToken + UserPermission
    // module) against one or more required tokens — the caller passes if it
    // holds ANY of the listed tokens, or the "ADMIN" superuser bypass token.
    //
    // Requires [Authorize] on the controller/action too — this attribute only
    // adds the granular permission check on top of base JWT authentication,
    // it does not authenticate by itself.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _tokens;

        public RequirePermAttribute(params string[] tokens)
        {
            _tokens = tokens;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var perms = context.HttpContext.User.Claims
                .Where(c => c.Type == "perms")
                .Select(c => c.Value)
                .ToList();

            if (perms.Contains("ADMIN"))
            {
                return;
            }

            if (!_tokens.Any(t => perms.Contains(t)))
            {
                context.Result = new ObjectResult(new
                {
                    errors = new[]
                    {
                        new { errorCode = "FORBIDDEN", errorMessage = "Bạn không có quyền thực hiện thao tác này." }
                    }
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}

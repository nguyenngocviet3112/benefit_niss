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
    [Authorize]
    [Route("api/usermodeaccess")]
    [ApiController]
    public class UserModeAccessController : ControllerBase
    {
        private readonly IUserModeAccessDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserModeAccessController(IUserModeAccessDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("HasAccess")]
        public IActionResult HasAccess()
        {
            UserModeAccessResponse response = new UserModeAccessResponse();
            try
            {
                RequestBaseDataContract request = new RequestBaseDataContract();
                request.GetHeaderInfo(Request.Headers);
                response.HasAccess = _dataManager.HasAccess(request.UserId);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("HasAccess", Log, null))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

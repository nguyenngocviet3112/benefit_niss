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
    [Route("api/usersync")]
    [ApiController]
    public class UserSyncController : ControllerBase
    {
        private readonly IUserSyncDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserSyncController(IUserSyncDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetInternalList")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult GetInternalList()
        {
            UserSyncListResponse response;
            try
            {
                response = _dataManager.GetInternalList();
            }
            catch (Exception e)
            {
                response = new UserSyncListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetInternalList", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetExternalList")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult GetExternalList()
        {
            UserSyncListResponse response;
            try
            {
                response = _dataManager.GetExternalList();
            }
            catch (Exception e)
            {
                response = new UserSyncListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetExternalList", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SyncInternal")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult SyncInternal(SyncInternalUserRequest request)
        {
            SyncInternalUserResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SyncInternal(request);
            }
            catch (Exception e)
            {
                response = new SyncInternalUserResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SyncInternal", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

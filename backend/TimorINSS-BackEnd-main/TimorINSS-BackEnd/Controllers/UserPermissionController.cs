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
    [Route("api/userpermission")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserPermissionController(IUserPermissionDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetCatalog")]
        public IActionResult GetCatalog()
        {
            PermissionCatalogResponse response;
            try
            {
                response = _dataManager.GetCatalog();
            }
            catch (Exception e)
            {
                response = new PermissionCatalogResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetCatalog", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetUsers")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult GetUsers()
        {
            UserPermissionListResponse response;
            try
            {
                response = _dataManager.GetUsers();
            }
            catch (Exception e)
            {
                response = new UserPermissionListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetUsers", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetUser/{id}")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult GetUser(int id)
        {
            UserPermissionDetailResponse response;
            try
            {
                GetUserPermissionRequest request = new GetUserPermissionRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Id = id;
                response = _dataManager.GetUser(request);
            }
            catch (Exception e)
            {
                response = new UserPermissionDetailResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetUser", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("SaveUser")]
        [RequirePerm("USER_MANAGE")]
        public IActionResult SaveUser(SaveUserPermissionRequest request)
        {
            SaveUserPermissionResponse response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveUser(request);
            }
            catch (Exception e)
            {
                response = new SaveUserPermissionResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("SaveUser", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

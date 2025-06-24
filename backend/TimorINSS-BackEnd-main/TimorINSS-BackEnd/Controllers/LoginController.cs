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
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUtilizadorDataManager _dataManager;
        private readonly IUtilsDataManager _utilsDataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoginController(IUtilizadorDataManager dataManager, IUtilsDataManager utilsDataManager)
        {
            _dataManager = dataManager;
            _utilsDataManager = utilsDataManager;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.LoginManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("Authenticate", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RecoverPassword")]
        public IActionResult RecoverPassword(RecoverRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.RecoverManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log no ficheiro de logs
            response.ManageErrors("RecoverPassword", Log, request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("FirstAcess")]
        public IActionResult FirstAcess(RecoverRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.FirstAcessManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log no ficheiro de logs
            response.ManageErrors("FirstAcess", Log, request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("SetUpPassword")]
        public IActionResult SetUpPassword(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SetUpPasswordManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SetUpPassword", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult CreateUser(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CreateUserManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("CreateUser", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("CreateNissInfor")]
        public IActionResult CreateNissInfor(CreateNissInforRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CreateNissInforManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("CreateNissInfor", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("InternalAuthenticate")]
        public IActionResult InternalAuthenticate(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.InternalLoginManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("InternalAuthenticate", Log, request))
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("Logs")]
        public IActionResult Logs(RequestBaseDataContract request)
        {
            LogResponse response = new LogResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _utilsDataManager.Compress(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.ToString() });
            }

            // Guardar log no ficheiro de logs
            response.ManageErrors("Logs", Log, request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("InternalFirstAcess")]
        public IActionResult InternalFirstAcess(RecoverRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.InternalFirstAcessManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log no ficheiro de logs
            response.ManageErrors("InternalFirstAcess", Log, request);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("InternalRecoverPassword")]
        public IActionResult InternalRecoverPassword(RecoverRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.InternalRecoverManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log no ficheiro de logs
            response.ManageErrors("InternalRecoverPassword", Log, request);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("CreateInternalUser")]
        public IActionResult CreateInternalUser(RecoverSetPasswordRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.CreateInternalUserManager(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("CreateInternalUser", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("ValidToken")]
        public IActionResult ValidToken(ValidTokenRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                response = _dataManager.ValidToken(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ValidToken", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
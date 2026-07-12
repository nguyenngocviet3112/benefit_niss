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
    // Tự phục vụ (self-service) — chỉ cần đăng nhập, KHÔNG cần RequirePerm
    // riêng: mỗi user luôn chỉ thao tác trên đúng hồ sơ của chính mình
    // (request.UserId lấy từ header User-Id, không nhận Id từ client).
    [Authorize]
    [Route("api/meuperfil")]
    [ApiController]
    public class MeuPerfilController : ControllerBase
    {
        private readonly IMeuPerfilDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MeuPerfilController(IMeuPerfilDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetProfile")]
        public IActionResult GetProfile()
        {
            MeuPerfilResponse response;
            try
            {
                GetMeuPerfilRequest request = new GetMeuPerfilRequest();
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetMeuPerfil(request);
            }
            catch (Exception e)
            {
                response = new MeuPerfilResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetProfile", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("AlterarEmail")]
        public IActionResult AlterarEmail(AlterarEmailRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AlterarEmail(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AlterarEmail", Log, request))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("AlterarSenha")]
        public IActionResult AlterarSenha(AlterarSenhaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AlterarSenha(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("AlterarSenha", Log, request))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

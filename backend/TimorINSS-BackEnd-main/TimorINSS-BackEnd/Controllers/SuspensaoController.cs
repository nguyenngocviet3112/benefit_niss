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
    [Route("api/suspensao")]
    [ApiController]
    public class SuspensaoController : ControllerBase
    {
        private readonly ISuspensaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SuspensaoController(ISuspensaoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // POST: api/suspensao
        [HttpPost("saveSuspensao")]
        public IActionResult SaveSuspensao(SuspensaoRequest request)
        {
            SuspensaoResponse response = new SuspensaoResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveSuspensao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("saveSuspensao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetByIdEntidadeEmpregadora")]
        public IActionResult GetByIdEntidadeEmpregadora([FromBody] SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse response = new SuspensaoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetSuspensaoByIdEntidadeEmpregadora(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetByIdEntidadeEmpregadora", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetByIdTrabalhador")]
        public IActionResult GetByIdTrabalhador([FromBody] SuspensaoListagemRequest request)
        {
            SuspensaoListagemResponse response = new SuspensaoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetSuspensaoByIdTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetByIdTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteSuspensao")]
        public IActionResult DeleteSuspensao(SuspensaoDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteSuspensao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteSuspensao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
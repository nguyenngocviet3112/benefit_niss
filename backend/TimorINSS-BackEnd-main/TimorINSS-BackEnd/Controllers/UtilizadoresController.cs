using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/utilizadores")]
    [ApiController]
    public class UtilizadoresController : ControllerBase
    {
        private readonly IUtilizadorDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UtilizadoresController(IUtilizadorDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetAllUtilizadoresInterno")]
        public IActionResult GetAllUtilizadoresInterno(SearchFilterRequest request)
        {
            UtilizadorListagemResponse response = new UtilizadorListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllUtilizadoresInterno(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllUtilizadoresInterno", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllDadosUtilizador")]
        public IActionResult GetAllDadosUtilizador(DadosUtilizadorRequest request)
        {
            DadosUtilizadorResponse response = new DadosUtilizadorResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllDadosUtilizador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllDadosUtilizador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("AddUtilizador")]
        public IActionResult AddUtilizador(UtilizadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.AddUtilizador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddUtilizador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllAcessoUtilizadores")]
        public IActionResult GetAllAcessoUtilizadores(SearchFilterRequest request)
        {
            UtilizadoresAcessoListagemResponse response = new UtilizadoresAcessoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllAcessoUtilizadores(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllAcessoUtilizadores", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("SwitchUserBlockState")]
        public IActionResult SwitchUserBlockState(UserUpdateRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SwitchUserBlockState(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SwitchUserBlockState", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
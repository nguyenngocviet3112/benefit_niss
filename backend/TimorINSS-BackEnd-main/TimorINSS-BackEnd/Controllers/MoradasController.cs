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
    [Route("api/morada")]
    [ApiController]
    public class MoradasController : ControllerBase
    {
        private readonly IMoradaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MoradasController(IMoradaDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetByIdEntidadeEmpregadora")]
        public IActionResult GetByIdEntidadeEmpregadora([FromBody] MoradaListagemRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetMoradasByIdEntidadeEmpregadora(request);
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
        public IActionResult GetMoradasByIdTrabalhador([FromBody] MoradaListagemRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetMoradasByIdTrabalhador(request);
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

        // POST: api/morada
        [HttpPost("SaveMorada")]
        public IActionResult SaveMorada(MoradaRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveMorada(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveMorada", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/morada
        [HttpPost("UpdateMorada")]
        public IActionResult UpdateMorada(MoradaRequest request)
        {
            MoradaListagemResponse response = new MoradaListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.UpdateMorada(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateMorada", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteMorada")]
        public IActionResult DeleteMorada(MoradaDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeleteMorada(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteMorada", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
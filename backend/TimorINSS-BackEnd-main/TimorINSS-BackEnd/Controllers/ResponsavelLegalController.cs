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
    [Route("api/responsavelLegal")]
    [ApiController]
    public class ResponsavelLegalController : ControllerBase
    {
        private IResponsavelLegalDataManager _responsavelLegalDataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ResponsavelLegalController(IResponsavelLegalDataManager responsavelLegalDataManager)
        {
            _responsavelLegalDataManager = responsavelLegalDataManager;
        }

        [HttpPost("AddReponsavelLegal")]
        public IActionResult AddReponsavelLegal(ResponsavelLegalRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _responsavelLegalDataManager.AddResponsavelLegal(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("AddReponsavelLegal", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("ListReponsavelLegal")]
        public IActionResult ListReponsavelLegal(ResponsavelLegalListagemRequest request)
        {
            SingleResponsavelLegalResponse response = new SingleResponsavelLegalResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _responsavelLegalDataManager.GetById(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("ListReponsavelLegal", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetByIdEntidadeEmpregadora")]
        public IActionResult GetByIdEntidadeEmpregadora([FromBody] ResponsavelLegalListagemRequest request)
        {
            ResponsavelLegalListagemResponse response = new ResponsavelLegalListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _responsavelLegalDataManager.GetByIdEntidadeEmpregadora(request);
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

        [HttpPost("UpdateResponsavelLegal")]
        public IActionResult UpdateResponsavelLegal(ResponsavelLegalRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _responsavelLegalDataManager.UpdateResponsavelLegal(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateResponsavelLegal", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteResponsavelLegal")]
        public IActionResult DeleteResponsavelLegal(ResponsavelLegalDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _responsavelLegalDataManager.DeleteResponsavelLegal(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteResponsavelLegal", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
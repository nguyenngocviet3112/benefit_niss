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
    [Route("api/contato")]
    [ApiController]
    public class ContatosController : ControllerBase
    {
        private readonly IContactoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ContatosController(IContactoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetByIdEntidadeEmpregadora")]
        public IActionResult GetByIdEntidadeEmpregadora([FromBody] ContatoListagemRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetContatosByIdEntidadeEmpregadora(request);
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
        public IActionResult GetByIdTrabalhador([FromBody] ContatoListagemRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetContatosByIdTrabalhador(request);
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

        // POST: api/contato
        [HttpPost("SaveContato")]
        public IActionResult SaveContato(ContatoRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveContato(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("SaveContato", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/contato
        [HttpPost("UpdateContato")]
        public IActionResult UpdateContato(ContatoRequest request)
        {
            ContatoListagemResponse response = new ContatoListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.UpdateContato(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateContato", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("DeleteContato")]
        public IActionResult DeleteContato(ContatoDeleteRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.DeleteContato(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("DeleteContato", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
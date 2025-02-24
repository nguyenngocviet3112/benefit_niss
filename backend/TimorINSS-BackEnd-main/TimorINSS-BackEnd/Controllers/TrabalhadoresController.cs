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
    [Route("api/trabalhadores")]
    [ApiController]
    public class TrabalhadoresController : ControllerBase
    {
        private readonly ITrabalhadorDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TrabalhadoresController(ITrabalhadorDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetByIdEntidadeEmpregadora")]
        public IActionResult GetByIdEntidadeEmpregadora([FromBody] TrabalhadorListagemRequest request)
        {
            TrabalhadorListagemResponse response = new TrabalhadorListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTrabalhadoresByIdEntidadeEmpregadora(request);
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

        [HttpPost("GetByFilter")]
        public IActionResult getsByFilter([FromBody] SearchFilterRequest request)
        {
            VincularTrabalhadorListagemResponse response = new VincularTrabalhadorListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.getTrabalhadoresByFilter(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetByFilter", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("saveTrabalhador")]
        public IActionResult SaveTrabalhador(TrabalhadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("saveTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/trabalhadores/getById
        [HttpPost("getById")]
        public IActionResult GetById(TrabalhadorListagemRequest request)
        {
            SingleTrabalhadorResponse response = new SingleTrabalhadorResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetById(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getById", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/trabalhadores/getTrabalhadorByNiss
        [HttpPost("getTrabalhadoresByNiss")]
        public IActionResult getTrabalhadoresByNiss(TrabalhadorListagemRequest request)
        {
            TrabalhadorListagemResponse response = new TrabalhadorListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTrabalhadoresByNiss(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getTrabalhadoresByNiss", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/trabalhadores/GetSingleByNiss
        [HttpPost("GetSingleByNiss")]
        public IActionResult GetSingleByNiss(TrabalhadorListagemNissRequest request)
        {
            TrabalhadorListagemResponse response = new TrabalhadorListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetSingleByNiss(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetSingleByNiss", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("editDadosPrincipais")]
        public IActionResult EditDadosPrincipais(EditTrabalhadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("editDadosPrincipais", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
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
    [Route("api/RelEntidadesTrabalhadores")]
    [ApiController]
    public class RelEntidadesTrabalhadoresController : ControllerBase
    {
        private readonly IRelEntidadeTrabalhadorDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RelEntidadesTrabalhadoresController(IRelEntidadeTrabalhadorDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("saveRelEntidadeTrabalhador")]
        public IActionResult SaveRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveRelEntidadeTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("saveRelEntidadeTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("editRelEntidadeTrabalhador")]
        public IActionResult EditRelEntidadeTrabalhador(RelEntidadeTrabalhadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditRelEntidadeTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("editRelEntidadeTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("editRelEntidadeTrabalhadorRegime")]
        public IActionResult EditRelEntidadeTrabalhadorRegime(RelEntidadeTrabalhadorRegimeRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditRelEntidadeTrabalhadorRegime(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("editRelEntidadeTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("desvincularTrabalhador")]
        public IActionResult DesvincularTrabalhador(DesvincularTrabalhadorRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DesvincularTrabalhador(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("desvincularTrabalhador", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("getTrabalhadorViewById")]
        public IActionResult GetTrabalhadorViewById(TrabalhadorListagemRequest request)
        {
            TrabalhadorViewResponse response = new TrabalhadorViewResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetTrabalhadorViewById(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("getTrabalhadorViewById", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
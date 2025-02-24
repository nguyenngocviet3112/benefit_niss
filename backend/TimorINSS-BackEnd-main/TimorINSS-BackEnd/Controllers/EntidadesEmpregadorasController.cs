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
    [Route("api/entidadeEmpregadora")]
    [ApiController]
    public class EntidadesEmpregadorasController : ControllerBase
    {
        private readonly IEntidadeEmpregadoraDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public EntidadesEmpregadorasController(IEntidadeEmpregadoraDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetByIdEntidade/{id}")]
        public IActionResult GetByIdEntidade(int id)
        {
            EntidadeEmpregadoraConsultaResponse response = new EntidadeEmpregadoraConsultaResponse();
            try
            {
                response = _dataManager.GetByIdEntidade(id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            return Ok(response);
        }

        // POST: api/entidadeEmpregadora
        [HttpPost("UpdateEntidade")]
        public IActionResult UpdateEntidade(EntidadeEmpregadoraRequest request)
        {
            EntidadeEmpregadoraConsultaResponse response = new EntidadeEmpregadoraConsultaResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.UpdateEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpdateEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("upsert")]
        public IActionResult UpsertEntidade(EntidadeEmpregadoraUpsertRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.UpsertEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("UpsertEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetEntidadeInfoForDeclaracao")]
        public IActionResult GetEntidadeInfoForDeclaracao(EntidadeEmpregadoraIdRequest request)
        {
            EntidadeEmpregadoraDeclaracaoViewResponse response = new EntidadeEmpregadoraDeclaracaoViewResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetEntidadeInfoForDeclaracao(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetEntidadeInfoForDeclaracao", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetEntidadeByNiss")]
        public IActionResult GetEntidadeByNiss(EntidadeEmpregadoraNissRequest request)
        {
            EntidadeEmpregadoraConsultaResponse response = new EntidadeEmpregadoraConsultaResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);

                response = _dataManager.GetEntidadeByNiss(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetEntidadeByNiss", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
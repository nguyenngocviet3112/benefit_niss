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
    [Route("api/contaCorrente")]
    [ApiController]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IContaCorrenteDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ContaCorrenteController(IContaCorrenteDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetContaCorrenteByIdEntidade")]
        public IActionResult GetContaCorrenteByIdEntidade([FromBody] ContaCorrenteListagemRequest request)
        {
            ContaCorrenteListagemResponse response = new ContaCorrenteListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetContaCorrenteByIdEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetContaCorrenteByIdEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("GetResumoContaCorrenteByIdEntidade")]
        public IActionResult GetResumoContaCorrenteByIdEntidade([FromBody] ResumoContaCorrenteListagemRequest request)
        {
            ResumoContaCorrenteListagemResponse response = new ResumoContaCorrenteListagemResponse();
            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetResumoContaCorrenteByIdEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetResumoContaCorrenteByIdEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/contaCorrente
        [HttpPost("GetAllContasStatesFromYearByFilter")]
        public IActionResult GetAllContasStatesFromYearByFilter(GetAllContasStatesFromYearByFilterRequest request)
        {
            GetAllContasStatesFromYearByFilterResponse response = new GetAllContasStatesFromYearByFilterResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetAllContasStatesFromYearByFilter(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllContasStatesFromYearByFilter", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
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
    [Route("api/reservaCredito")]
    [ApiController]
    public class ReservaCreditoController : ControllerBase
    {
        private readonly IReservaCreditoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ReservaCreditoController(IReservaCreditoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetReservaCreditoByIdEntidade")]
        public IActionResult GetReservaCreditoByIdEntidade([FromBody] ReservaCreditoListagemRequest request)
        {
            ReservaCreditoListagemResponse response = new ReservaCreditoListagemResponse();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetReservaCreditoByIdEntidade(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetReservaCreditoByIdEntidade", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
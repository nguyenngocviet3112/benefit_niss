using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Authorize]
    [Route("api/relUtilizadorDepartamento")]
    [ApiController]
    public class RelUtilizadorDepartamentosController : ControllerBase
    {
        private readonly IRelUtilizadorDepartamentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RelUtilizadorDepartamentosController(IRelUtilizadorDepartamentoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetDepartamentosByUserId")]
        public IActionResult GetDepartamentosByUserId([FromBody] int id)
        {
            DepartamentoListagemResponse response = new DepartamentoListagemResponse();

            try
            {
                response = _dataManager.GetDepartamentosByUserId(id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetDepartamentosByUserId", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
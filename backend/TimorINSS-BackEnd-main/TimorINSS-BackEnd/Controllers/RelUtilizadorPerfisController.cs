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
    [Route("api/relUtilizadorPerfil")]
    [ApiController]
    public class RelUtilizadorPerfisController : ControllerBase
    {
        private readonly IRelUtilizadorPerfilDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RelUtilizadorPerfisController(IRelUtilizadorPerfilDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetPerfisByUserId")]
        public IActionResult GetPerfisByUserId([FromBody] int id)
        {
            PerfilListagemResponse response = new PerfilListagemResponse();

            try
            {
                response = _dataManager.GetPerfisByUserId(id);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetPerfisByUserId", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
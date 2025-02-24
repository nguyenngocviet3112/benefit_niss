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
    [Route("api/relPerfilFuncionalidade")]
    [ApiController]
    public class RelPerfisFuncionalidadesController : ControllerBase
    {
        private readonly IRelPerfilFuncionalidadeDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RelPerfisFuncionalidadesController(IRelPerfilFuncionalidadeDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetRelPerfilFuncionalidadeByIdPerfil")]
        public IActionResult GetRelPerfilFuncionalidadeByIdPerfil([FromBody] int idPerfil)
        {
            FuncionalidadeListagemResponse response = new FuncionalidadeListagemResponse();

            try
            {
                response = _dataManager.GetRelPerfilFuncionalidadeByIdPerfil(idPerfil);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetRelPerfilFuncionalidadeByIdPerfil", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
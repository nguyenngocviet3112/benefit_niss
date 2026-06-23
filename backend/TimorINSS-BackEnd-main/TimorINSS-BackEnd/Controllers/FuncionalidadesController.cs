using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Route("api/funcionalidade")]
    [ApiController]
    public class FuncionalidadesController : ControllerBase
    {
        private readonly IFuncionalidadeDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FuncionalidadesController(IFuncionalidadeDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetAllFuncionalidades")]
        public IActionResult GetAllFuncionalidades()
        {
            FuncionalidadeListagemResponse response = new FuncionalidadeListagemResponse();

            try
            {
                response = _dataManager.GetAllFuncionalidades();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("GetAllFuncionalidades", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
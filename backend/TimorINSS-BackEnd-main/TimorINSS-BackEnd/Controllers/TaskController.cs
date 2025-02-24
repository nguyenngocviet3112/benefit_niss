using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TaskController(ITaskDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        //[AllowAnonymous]
        //[HttpGet("UpdateContaCorrente")]
        //public IActionResult UpdateContaCorrente()
        //{
        //    ResponseBaseDataContract response = new ResponseBaseDataContract();
        //    try
        //    {
        //        response = _dataManager.UpdateContaCorrente(DateTime.Now);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
        //    }
        //    // Guardar log no ficheiro de logs
        //    response.ManageErrors("UpdateContaCorrente", Log, null);
        //    return Ok();
        //}

        //[AllowAnonymous]
        //[HttpGet("UpdateGuiaPagamento")]
        //public IActionResult UpdateGuiaPagamento()
        //{
        //    ResponseBaseDataContract response = new ResponseBaseDataContract();
        //    try
        //    {
        //        response = _dataManager.UpdateGuiaPagamento();
        //    }
        //    catch (Exception e)
        //    {
        //        response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
        //    }
        //    // Guardar log no ficheiro de logs
        //    response.ManageErrors("UpdateGuiaPagamento", Log, null);
        //    return Ok();
        //}

        [AllowAnonymous]
        [HttpGet("UpdateDeclaracao")]
        public IActionResult UpdateDeclaracao()
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                response = _dataManager.UpdateDeclaracao();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log no ficheiro de logs
            response.ManageErrors("UpdateDeclaracao", Log, null);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("UpdateTrabalhadorInterno")]
        public IActionResult UpdateTrabalhadorInterno()
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                response = _dataManager.UpdateTrabalhadorInterno();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log no ficheiro de logs
            response.ManageErrors("UpdateTrabalhadorInterno", Log, null);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("DeleteOldImports")]
        public IActionResult DeleteOldImports()
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();
            try
            {
                _dataManager.DeleteOldImports();
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }
            // Guardar log no ficheiro de logs
            response.ManageErrors("DeleteOldImports", Log, null);
            return Ok();
        }
    }
}
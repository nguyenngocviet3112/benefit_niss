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
    [Route("api/INSSEstrangeiros")]
    [ApiController]
    public class INSSEstrangeiroController : ControllerBase
    {
        private readonly IINSSEstrangeiroDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public INSSEstrangeiroController(IINSSEstrangeiroDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("editINSSEstrangeiro")]
        public IActionResult EditINSSEstrangeiro(INSSEstrangeiroRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.EditINSSEstrangeiro(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("editINSSEstrangeiro", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // POST: api/RelEntidadesTrabalhadores
        [HttpPost("saveINSSEstrangeiro")]
        public IActionResult SaveINSSEstrangeiro(INSSEstrangeiroRequest request)
        {
            ResponseBaseDataContract response = new ResponseBaseDataContract();

            try
            {
                // Parse dos valores do header para o request
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveINSSEstrangeiro(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            // Guardar log do erro no ficheiro de logs
            if (response.ManageErrors("saveINSSEstrangeiro", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
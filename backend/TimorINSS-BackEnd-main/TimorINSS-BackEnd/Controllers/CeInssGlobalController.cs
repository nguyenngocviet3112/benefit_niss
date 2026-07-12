using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    // CE_INSS_Global — priority M4 report (finance-brd §6.15): global Receita+Despesa
    // summary grouped only by Classificação Económica, no Programa/Atividade/Regime breakdown.
    [Authorize]
    [Route("api/ceInssGlobal")]
    [ApiController]
    public class CeInssGlobalController : ControllerBase
    {
        private readonly ICeInssGlobalDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CeInssGlobalController(ICeInssGlobalDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("GetReport")]
        [RequirePerm("REPORT_VIEW")]
        public IActionResult GetReport(CeInssGlobalRequest request)
        {
            CeInssGlobalResponse response = new CeInssGlobalResponse();
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetReport(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new TimorINSSBackEnd.DataContracts.ResponseDataContract.Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetReport", Log, request))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}

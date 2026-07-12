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
    // New-mode-only controller: matches declared Guia de Pagamento against real bank
    // statement lines before promoting to Paid. Reuses "BANCO_CONCILIAR" (already the
    // token for the Conciliação de Movimentos screen) since this is the same activity
    // applied to a different money-in source.
    [Authorize]
    [Route("api/guiaConciliacao")]
    [ApiController]
    public class GuiaConciliacaoController : ControllerBase
    {
        private readonly IGuiaConciliacaoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GuiaConciliacaoController(IGuiaConciliacaoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost("ConciliarGuiaPagamento")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult ConciliarGuiaPagamento(ConciliarGuiaPagamentoRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.ConciliarGuiaPagamento(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("ConciliarGuiaPagamento", Log, request))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetComprovativo/{idGuia}")]
        [RequirePerm("BANCO_CONCILIAR")]
        public IActionResult GetComprovativo(int idGuia)
        {
            GuiaComprovativoResponse response;
            try
            {
                var request = new GetGuiaComprovativoRequest { IdGuia = idGuia };
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.GetComprovativo(request);
            }
            catch (Exception e)
            {
                response = new GuiaComprovativoResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetComprovativo", Log, null))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

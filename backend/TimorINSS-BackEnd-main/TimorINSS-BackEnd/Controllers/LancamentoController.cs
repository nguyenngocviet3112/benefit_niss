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
    // Chỉ đọc — bút toán được tự sinh bởi Pagamento/Receita, không có
    // Save/Deactivate thủ công ở đây (xem LancamentoDataManager).
    [Authorize]
    [Route("api/lancamento")]
    [ApiController]
    public class LancamentoController : ControllerBase
    {
        private readonly ILancamentoDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LancamentoController(ILancamentoDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] string origemTipo)
        {
            LancamentoListResponse response;
            try
            {
                GetLancamentoListRequest request = new GetLancamentoListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                request.Mes = mes;
                request.OrigemTipo = origemTipo;
                response = _dataManager.GetList(request);
            }
            catch (Exception e)
            {
                response = new LancamentoListResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetList", Log, null))
                return BadRequest(response);

            return Ok(response);
        }
    }
}

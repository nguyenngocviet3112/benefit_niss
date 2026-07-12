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
    // Ciclo da Despesa — báo cáo M4, 1 dòng/AD, theo dõi toàn bộ chu trình chấp hành chi tiêu
    // (Cabimento -> Compromisso -> Obrigação -> Pagamento) kèm saldo từng bước, đúng cấu trúc
    // sheet "Ciclo_Despesa" gốc (INSS_2026_janeiro _original.xlsx).
    [Authorize]
    [Route("api/cicloDespesa")]
    [ApiController]
    public class CicloDespesaController : ControllerBase
    {
        private readonly ICicloDespesaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CicloDespesaController(ICicloDespesaDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetByAno/{ano}")]
        [RequirePerm("REPORT_VIEW")]
        public IActionResult GetByAno(int ano, [FromQuery] int? institution)
        {
            CicloDespesaListResponse response = new CicloDespesaListResponse();
            try
            {
                GetCicloDespesaListRequest request = new GetCicloDespesaListRequest();
                request.GetHeaderInfo(Request.Headers);
                request.Ano = ano;
                request.Institution = institution;
                response = _dataManager.GetByAno(request);
            }
            catch (Exception e)
            {
                response.Errors.Add(new TimorINSSBackEnd.DataContracts.ResponseDataContract.Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetByAno", Log, null))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}

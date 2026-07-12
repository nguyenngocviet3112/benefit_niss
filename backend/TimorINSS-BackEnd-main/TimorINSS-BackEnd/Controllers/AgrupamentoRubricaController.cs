using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.Cache;
using TimorINSSBackEnd.DataContracts.RequestDataContract;
using TimorINSSBackEnd.DataContracts.ResponseDataContract;
using TimorINSSBackEnd.DataManager.Interfaces;

namespace TimorINSSBackEnd.Controllers
{
    // "Mapeamento Rubricas" (mode mới) — quản lý Agrupamentoconfig CHỈ cho 4 TipoConta
    // phục vụ đúng nhu cầu mapping Codigoconta -> dòng Balanço/DR: Receita/Despesa/
    // Neutro Receita/Neutro Despesa (idDominio 70/71/72/73, verify qua DB thật 2026-07-12).
    //
    // CỐ Ý loại trừ 2 TipoConta khác cũng tồn tại trong bảng này — "Actidade" (1221, 20
    // dòng) và "Funcional" (1223, 80 dòng) — vì kiểm tra trực tiếp cho thấy nội dung của
    // chúng TRÙNG LẶP hoàn toàn với cây Programa/Subprograma/Atividade và
    // Classificação Funcional (COFOG) đã có sẵn trong ProgramActivity/
    // FunctionalClassification (2 bảng mode mới, purpose-built, có CRUD riêng). Đây là dữ
    // liệu cũ tạo ra TRƯỚC KHI 2 bảng đó tồn tại — không phải yêu cầu từ Excel/luật, nên
    // không đưa vào màn quản lý mới này (không xoá, chỉ không hiển thị/không quản lý ở đây).
    //
    // Tách biệt hoàn toàn khỏi AgrupamentoConfigController đã có (mục đích khác, không sửa).
    [Authorize]
    [Route("api/agrupamentorubrica")]
    [ApiController]
    public class AgrupamentoRubricaController : ControllerBase
    {
        private readonly IAgrupamentoRubricaDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICacheProvider _cache;

        public AgrupamentoRubricaController(IAgrupamentoRubricaDataManager dataManager, ICacheProvider memoryCache)
        {
            _dataManager = dataManager;
            _cache = memoryCache;
        }

        [HttpGet("GetTree/{orcamentoConfigFk}/{tipoConta}")]
        public IActionResult GetTree(int orcamentoConfigFk, string tipoConta)
        {
            AgrupamentoRubricaTreeResponse response;
            try
            {
                GetAgrupamentoRubricaTreeRequest request = new GetAgrupamentoRubricaTreeRequest();
                request.GetHeaderInfo(Request.Headers);
                request.OrcamentoConfigFk = orcamentoConfigFk;
                request.TipoConta = tipoConta;
                response = _dataManager.GetTreeByOrcamentoConfig(request);
            }
            catch (Exception e)
            {
                response = new AgrupamentoRubricaTreeResponse();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("GetTree", Log, null))
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Save")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Save(SaveAgrupamentoRubricaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.SaveAgrupamentoRubrica(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Save", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }

        [HttpPost("Deactivate")]
        [RequirePerm("MASTERDATA_MANAGE")]
        public IActionResult Deactivate(DeactivateAgrupamentoRubricaRequest request)
        {
            ResponseBaseDataContract response;
            try
            {
                request.GetHeaderInfo(Request.Headers);
                response = _dataManager.DeactivateAgrupamentoRubrica(request);
            }
            catch (Exception e)
            {
                response = new ResponseBaseDataContract();
                response.Errors.Add(new Error { ErrorCode = "-1", ErrorMessage = e.Message });
            }

            if (response.ManageErrors("Deactivate", Log, request))
                return BadRequest(response);

            _cache.Reset();
            return Ok(response);
        }
    }
}

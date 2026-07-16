using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using TimorINSSBackEnd.DataManager.Interfaces;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Controllers
{
    // Bộ API cấp dữ liệu cho module Benefit (inss-benefit-app) consume qua Connector
    // CONTRIB_API — đặc tả đầy đủ theo BRD Benefit module §18 "TÍCH HỢP API CONTRIBUTION"
    // (base URL kỳ vọng: http://<host>:5000/api/benefit-data). File hoàn toàn mới, tách
    // biệt khỏi BenefitController/api/benefit hiện có (getCitizenByInss) — không sửa gì ở
    // đó. Khách hàng cuối tự quyết định có pull nhóm file này về hay không; nếu không dùng,
    // xoá 5 file: BenefitDataController.cs, IBenefitDataDataManager.cs,
    // BenefitDataDataManager.cs, IBenefitDataRepository.cs, BenefitDataRepository.cs,
    // BenefitDataResponseDataContract.cs, và bỏ 2 dòng đăng ký DI trong ServiceExtensions.cs.
    //
    // [AllowAnonymous] theo đúng BRD ("Local: AllowAnonymous") — khi triển khai thật, PHẢI
    // thêm bảo vệ (API key / IP allow-list) trước khi expose ra ngoài, như BRD/DEPLOY.md của
    // Benefit đã lưu ý ở mục "Security".
    [AllowAnonymous]
    [Route("api/benefit-data")]
    [ApiController]
    public class BenefitDataController : ControllerBase
    {
        private readonly IBenefitDataDataManager _dataManager;
        public readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BenefitDataController(IBenefitDataDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // Hàm 1 — GET /api/benefit-data/company/{nissCompany}
        [HttpGet("company/{nissCompany}")]
        public IActionResult GetCompany(string nissCompany)
        {
            try
            {
                var response = _dataManager.GetCompanyMasterByNiss(nissCompany);
                if (!response.found)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"BenefitData.GetCompany({nissCompany})", e);
                return StatusCode(500, new { error = e.Message });
            }
        }

        // Hàm 2 — GET /api/benefit-data/worker/{niss}
        [HttpGet("worker/{niss}")]
        public IActionResult GetWorker(string niss)
        {
            try
            {
                var response = _dataManager.GetWorkerFullByNiss(niss);
                if (!response.found)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"BenefitData.GetWorker({niss})", e);
                return StatusCode(500, new { error = e.Message });
            }
        }

        // Hàm 2b — GET /api/benefit-data/document/{idDoc}
        [HttpGet("document/{idDoc}")]
        public IActionResult GetDocument(int idDoc)
        {
            try
            {
                Documentoidentificacao doc = _dataManager.GetDocumentoById(idDoc);
                if (doc == null || doc.Documento == null || doc.Documento.Length == 0)
                {
                    return NotFound();
                }

                // Header HTTP chỉ chấp nhận ASCII (Kestrel chặn cứng) — Descricao/Numero có thể
                // chứa dấu tiếng Bồ (Certidão, Eleição...); bỏ dấu trước khi set. Bản đầy đủ có
                // dấu vẫn lấy được qua GET /worker/{niss} -> documentos[].tipoDesc.
                Response.Headers.Add("X-Document-Type-Fk", doc.TpDocIdentificacao.ToString());
                Response.Headers.Add("X-Document-Type", ToAsciiHeaderSafe(doc.TpDocIdentificacaoNavigation?.Descricao));
                Response.Headers.Add("X-Document-Type-EN", ToAsciiHeaderSafe(doc.TpDocIdentificacaoNavigation?.DescricaoEn));
                Response.Headers.Add("X-Document-Number", ToAsciiHeaderSafe(doc.Numero));

                return File(doc.Documento, "application/pdf", doc.NomeDocumento);
            }
            catch (Exception e)
            {
                Log.Error($"BenefitData.GetDocument({idDoc})", e);
                return StatusCode(500, new { error = e.Message });
            }
        }

        // Hàm 3 — GET /api/benefit-data/contributions/{niss}
        [HttpGet("contributions/{niss}")]
        public IActionResult GetContributions(string niss)
        {
            try
            {
                var response = _dataManager.GetContributionHistoryByNiss(niss);
                if (!response.found)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"BenefitData.GetContributions({niss})", e);
                return StatusCode(500, new { error = e.Message });
            }
        }

        private static string ToAsciiHeaderSafe(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            string normalized = value.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c < 128)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}

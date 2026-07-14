using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using TimorINSSBackEnd.Authorization;
using TimorINSSBackEnd.Models;

namespace TimorINSSBackEnd.Controllers
{
    /// <summary>
    /// API ĐỌC THÔ (raw-only) phục vụ Benefit module.
    /// Nguyên tắc: Contribution = single source of truth (chỉ trả dữ liệu gốc),
    /// KHÔNG tính média/thâm niên/đủ-điều-kiện/số tiền hưu. Mọi công thức nằm ở Benefit engine.
    /// LƯU Ý BẢO MẬT: [AllowAnonymous] để bỏ qua lớp JWT/base64 obfuscation (server-to-server nội bộ).
    /// Production: giới hạn bằng network/API-key trước khi mở ra ngoài.
    /// </summary>
    [AllowAnonymous]
    [Route("api/benefit-data")]
    [ApiController]
    public class BenefitDataController : ControllerBase
    {
        private readonly TimorINSSModuloContribuicoesContext _ctx;

        public BenefitDataController(TimorINSSModuloContribuicoesContext ctx)
        {
            _ctx = ctx;
        }

        // ---------- Hàm 1: Thông tin công ty theo NISS công ty ----------
        [HttpGet("company/{niss}")]
        [RequireBenefitApiEnabled]
        public IActionResult GetCompany(string niss)
        {
            const string sql = @"
SELECT e.NISS, e.nome, e.TIN, e.dataInicioActiv, e.dtInscricao, e.numTrabalhador,
       m.rua, m.numPorta, m.morada_aldeia_fk AS aldeiaFk,
       c.telemovel, c.email
FROM ENTIDADEEMPREGADORA e
OUTER APPLY (SELECT TOP 1 rua, numPorta, morada_aldeia_fk FROM MORADA
             WHERE entidade_morada_fk = e.idEntidadeEmpreg
             ORDER BY CASE WHEN moradaPrincipal = 1 THEN 0 ELSE 1 END, idMorada) m
OUTER APPLY (SELECT TOP 1 telemovel, email FROM CONTACTO
             WHERE contacto_entidade_fk = e.idEntidadeEmpreg
             ORDER BY idContacto) c
WHERE e.NISS = @niss;";

            var rows = Query(sql, ("@niss", niss));
            if (rows.Count == 0)
                return NotFound(new { message = "Company NISS not found", niss });
            return Ok(rows[0]);
        }

        // ---------- Hàm 2: HỒ SƠ ĐẦY ĐỦ của NLĐ theo NISS (master + giấy tờ + địa chỉ + liên hệ + INSS nước ngoài) ----------
        [HttpGet("worker/{niss}")]
        [RequireBenefitApiEnabled]
        public IActionResult GetWorker(string niss)
        {
            // Trả kèm text (descricao) cho sexo/estadoCivil/nacionalidade — vì mã DOMINIO có thể
            // đổi giữa các bản DB (vd Viuvo 1110->1111). Benefit nên dùng *Desc thay vì hardcode mã.
            const string masterSql = @"
SELECT t.NISS, t.nome, t.dataNasc, t.naturalidade,
       t.sexoTrabalhador AS sexoFk, dsex.descricao AS sexoDesc, dsex.descricaoEN AS sexoDescEN,
       t.estadoCivil AS estadoCivilFk, dciv.descricao AS estadoCivilDesc, dciv.descricaoEN AS estadoCivilDescEN,
       t.nacionalidadeTrabalhador AS nacionalidadeFk, dnac.descricao AS nacionalidadeDesc, dnac.descricaoEN AS nacionalidadeDescEN,
       t.TIN, t.numInscProvisoria,
       t.nomeMae, t.indDescNomeMae, t.nomePai, t.indDescNomePai, t.interno
FROM TRABALHADOR t
LEFT JOIN DOMINIO dsex ON dsex.idDominio = t.sexoTrabalhador
LEFT JOIN DOMINIO dciv ON dciv.idDominio = t.estadoCivil
LEFT JOIN DOMINIO dnac ON dnac.idDominio = t.nacionalidadeTrabalhador
WHERE t.NISS = @niss;";

            var master = Query(masterSql, ("@niss", niss));
            if (master.Count == 0)
                return NotFound(new { message = "Worker NISS not found", niss });

            const string docsSql = @"
SELECT doc.idDocIdentificacao AS idDoc, doc.tpDocIdentificacao AS tipoFk,
       dt.descricao AS tipoDesc, dt.descricaoEN AS tipoDescEN,
       doc.numero, doc.dataEmissao, doc.dataValidade, doc.localEmissao,
       doc.nomeDocumento AS fileName,
       CASE WHEN doc.documento IS NOT NULL AND DATALENGTH(doc.documento) > 0 THEN 1 ELSE 0 END AS hasFile
FROM TRABALHADOR t
JOIN DOCUMENTOIDENTIFICACAO doc ON doc.trabalhador_documeto_fk = t.idTrabalhador AND doc.indActivo = 1
LEFT JOIN DOMINIO dt ON dt.idDominio = doc.tpDocIdentificacao
WHERE t.NISS = @niss
ORDER BY doc.idDocIdentificacao;";

            // Địa chỉ NLĐ + chuỗi địa giới đọc được (Aldeia->Suco->Posto->Município) + País
            const string moradasSql = @"
SELECT m.idMorada, m.rua, m.numPorta, m.moradaPrincipal,
       m.morada_aldeia_fk AS aldeiaFk, a.nome AS aldeia,
       s.idSuco AS sucoFk, s.nome AS suco,
       p.idPostoAdmin AS postoFk, p.nome AS posto,
       mu.idMunicipio AS municipioFk, mu.nome AS municipio,
       m.morada_pais_fk AS paisFk, pa.nome AS pais
FROM TRABALHADOR t
JOIN MORADA m ON m.trabalhador_morada_fk = t.idTrabalhador
LEFT JOIN ALDEIA a ON a.idAldeia = m.morada_aldeia_fk
LEFT JOIN SUCO s ON s.idSuco = a.aldeia_suco_fk
LEFT JOIN POSTOADMINISTRATIVO p ON p.idPostoAdmin = s.suco_postoAdmin_fk
LEFT JOIN MUNICIPIO mu ON mu.idMunicipio = p.postoAdmin_municipio_fk
LEFT JOIN PAIS pa ON pa.idPais = m.morada_pais_fk
WHERE t.NISS = @niss
ORDER BY m.moradaPrincipal DESC, m.idMorada;";

            const string contactosSql = @"
SELECT c.idContacto, c.telemovel, c.email, c.indActivo
FROM TRABALHADOR t
JOIN CONTACTO c ON c.contacto_trabalhador_fk = t.idTrabalhador
WHERE t.NISS = @niss
ORDER BY c.idContacto;";

            const string estrangeiroSql = @"
SELECT ie.idINSSEstrang AS id, ie.nomeSSEstrangeiro, ie.estrangeiro_pais_fk AS paisFk, pa.nome AS pais,
       ie.NISSEstrangeiro, ie.indDecontAtualmente, ie.indBenfAtualmente, ie.nomeDocumento,
       CASE WHEN ie.documento IS NOT NULL AND DATALENGTH(ie.documento) > 0 THEN 1 ELSE 0 END AS hasFile
FROM TRABALHADOR t
JOIN INSSESTRANGEIRO ie ON ie.estrangeiro_trabalhador_fk = t.idTrabalhador AND ie.indActivo = 1
LEFT JOIN PAIS pa ON pa.idPais = ie.estrangeiro_pais_fk
WHERE t.NISS = @niss
ORDER BY ie.idINSSEstrang;";

            var result = master[0];
            result["documentos"] = Query(docsSql, ("@niss", niss));
            result["moradas"] = Query(moradasSql, ("@niss", niss));
            result["contactos"] = Query(contactosSql, ("@niss", niss));
            result["inssEstrangeiro"] = Query(estrangeiroSql, ("@niss", niss));
            return Ok(result);
        }

        // ---------- Tải file PDF giấy tờ của worker theo idDoc (lấy từ Hàm 2) ----------
        [HttpGet("document/{idDoc:int}")]
        [RequireBenefitApiEnabled]
        public IActionResult GetDocumentFile(int idDoc)
        {
            const string sql = @"SELECT doc.documento, doc.nomeDocumento, doc.numero,
       doc.tpDocIdentificacao AS tipoFk, dm.descricao AS tipoDesc, dm.descricaoEN AS tipoDescEN
FROM DOCUMENTOIDENTIFICACAO doc
LEFT JOIN DOMINIO dm ON dm.idDominio = doc.tpDocIdentificacao
WHERE doc.idDocIdentificacao = @id AND doc.indActivo = 1;";
            var rows = Query(sql, ("@id", idDoc));
            if (rows.Count == 0 || rows[0]["documento"] == null)
                return NotFound(new { message = "Document not found or empty", idDoc });

            var row = rows[0];
            var bytes = (byte[])row["documento"];
            var name = row["nomeDocumento"] as string;
            if (string.IsNullOrWhiteSpace(name)) name = "documento_" + idDoc + ".pdf";

            // Loại giấy tờ đi kèm file qua header (tên file KHÔNG đáng tin để suy ra loại).
            // URL-encode để an toàn với ký tự non-ASCII (vd "Cartão"); client decodeURIComponent.
            Response.Headers["X-Document-Type-Fk"] = (row["tipoFk"]?.ToString()) ?? "";
            Response.Headers["X-Document-Type"] = Uri.EscapeDataString((row["tipoDesc"] as string) ?? "");
            Response.Headers["X-Document-Type-EN"] = Uri.EscapeDataString((row["tipoDescEN"] as string) ?? "");
            Response.Headers["X-Document-Number"] = Uri.EscapeDataString((row["numero"] as string) ?? "");
            return File(bytes, "application/pdf", name);
        }

        // ---------- Hàm 3: Lịch sử đóng góp theo NISS NLĐ, lồng theo từng hợp đồng ----------
        [HttpGet("contributions/{niss}")]
        [RequireBenefitApiEnabled]
        public IActionResult GetContributions(string niss)
        {
            const string workerSql = "SELECT NISS, nome FROM TRABALHADOR WHERE NISS = @niss;";
            var worker = Query(workerSql, ("@niss", niss));
            if (worker.Count == 0)
                return NotFound(new { message = "Worker NISS not found", niss });

            // (1) Mức hợp đồng — 1 dòng / idRel (cùng công ty có thể nhiều HĐ)
            // Kèm text cho tipoContrato/regime/profissao (regime tra bảng REGIME, 2 cái kia tra DOMINIO).
            // Benefit nên dùng *Desc thay vì hardcode mã.
            const string contractsSql = @"
SELECT r.idRel, e.NISS AS nissCompany, e.nome AS nomeCompany,
       r.dtIniVincTrabalhador, r.dtIniFimTrabalhador,
       r.tipoContrato AS tipoContratoFk, dtc.descricao AS tipoContratoDesc, dtc.descricaoEN AS tipoContratoDescEN,
       r.regime_fk AS regimeFk, drg.descricao AS regimeDesc, drg.descricaoEN AS regimeDescEN,
       r.profissao AS profissaoFk, dpr.descricao AS profissaoDesc, dpr.descricaoEN AS profissaoDescEN,
       MIN(d.mesAno) AS firstContribMonth, MAX(d.mesAno) AS lastContribMonth
FROM TRABALHADOR t
JOIN RELENTIDADETRABALHADOR r ON r.trabalhador_fk = t.idTrabalhador
JOIN ENTIDADEEMPREGADORA    e ON e.idEntidadeEmpreg = r.entidade_fk
LEFT JOIN DOMINIO dtc ON dtc.idDominio = r.tipoContrato
LEFT JOIN DOMINIO drg ON drg.idDominio = r.regime_fk
LEFT JOIN DOMINIO dpr ON dpr.idDominio = r.profissao
LEFT JOIN DECLARACAOREMUNERACAO d ON d.declaracao_relEntidadeTrabalhador_FK = r.idRel AND d.indActivo = 1
WHERE t.NISS = @niss
GROUP BY r.idRel, e.NISS, e.nome, r.dtIniVincTrabalhador, r.dtIniFimTrabalhador,
         r.tipoContrato, dtc.descricao, dtc.descricaoEN,
         r.regime_fk, drg.descricao, drg.descricaoEN,
         r.profissao, dpr.descricao, dpr.descricaoEN
ORDER BY r.dtIniVincTrabalhador;";

            // (2) Mức tháng — gắn vào HĐ theo idRel
            // NGUYÊN TẮC: chỉ trả số DOANH NGHIỆP ĐÃ KHAI BÁO, KHÔNG tự tính.
            // months[] chỉ gồm tháng thực có declaração (không suy diễn theo độ dài HĐ).
            // remunDeclarada/decimoTerceiro/dias = số khai báo gốc. regimeFk/taxa* = master-data tham chiếu
            // (để benefit TỰ tính tiền đóng nếu cần) — API KHÔNG nhân sẵn số tiền.
            const string monthsSql = @"
SELECT d.declaracao_relEntidadeTrabalhador_FK AS idRel,
       d.mesAno, d.remunDeclarada, d.decimoTerceiro,
       d.RegimeFK AS regimeFk, rg.nomeRegime AS regimeDesc,
       rg.percentEntidadeEmpreg AS taxaEntidade, rg.percentTrabalhador AS taxaTrabalhador,
       d.diasTrabcontabSegSocial AS diasTrabSegSocial, d.diasContrato, d.diasEfecTrabalhados,
       d.faltasInjustific, d.diasParentalidade, d.oficioso
FROM TRABALHADOR t
JOIN RELENTIDADETRABALHADOR r ON r.trabalhador_fk = t.idTrabalhador
JOIN DECLARACAOREMUNERACAO  d ON d.declaracao_relEntidadeTrabalhador_FK = r.idRel
LEFT JOIN REGIME rg ON rg.idRegime = d.RegimeFK
WHERE t.NISS = @niss AND d.indActivo = 1
ORDER BY idRel, d.mesAno;";

            // (3) Kỳ tạm dừng thô — gắn vào HĐ theo công ty + NLĐ
            const string suspSql = @"
SELECT e.NISS AS nissCompany, s.dataInicioSuspensao AS dataInicio, s.dataFimSuspensao AS dataFim
FROM SUSPENSOES s
JOIN TRABALHADOR t ON t.idTrabalhador = s.trabalhador_suspensao_fk
LEFT JOIN ENTIDADEEMPREGADORA e ON e.idEntidadeEmpreg = s.entidade_suspensao_fk
WHERE t.NISS = @niss AND s.indActivo = 1;";

            var contracts = Query(contractsSql, ("@niss", niss));
            var months = Query(monthsSql, ("@niss", niss));
            var susps = Query(suspSql, ("@niss", niss));

            // Gắn months[] + suspensions[] vào từng hợp đồng (idRel)
            foreach (var ct in contracts)
            {
                var idRel = Convert.ToInt64(ct["idRel"]);
                var nissCompany = ct["nissCompany"] as string;

                var ctMonths = new List<Dictionary<string, object>>();
                foreach (var m in months)
                {
                    if (Convert.ToInt64(m["idRel"]) == idRel)
                    {
                        m["status"] = "CONTRIBUTED";
                        ctMonths.Add(m);
                    }
                }
                ct["months"] = ctMonths;

                var ctSusp = new List<Dictionary<string, object>>();
                foreach (var s in susps)
                    if ((s["nissCompany"] as string) == nissCompany) ctSusp.Add(s);
                ct["suspensions"] = ctSusp;
            }

            // Gom 2 tầng: CÔNG TY -> các HỢP ĐỒNG -> tháng. Kèm tổng số tháng + tổng giá trị / công ty.
            // (totalMonths/totalRemunDeclarada chỉ là TIỆN ÍCH HIỂN THỊ; Benefit engine tự tính lại từ months[])
            var companies = new List<Dictionary<string, object>>();
            var byNiss = new Dictionary<string, Dictionary<string, object>>();
            foreach (var ct in contracts)
            {
                var nissCompany = (ct["nissCompany"] as string) ?? "";
                if (!byNiss.TryGetValue(nissCompany, out var comp))
                {
                    comp = new Dictionary<string, object>
                    {
                        ["nissCompany"] = ct["nissCompany"],
                        ["nomeCompany"] = ct["nomeCompany"],
                        ["numContracts"] = 0,
                        ["totalMonths"] = 0,
                        ["totalRemunDeclarada"] = 0m,
                        ["contracts"] = new List<Dictionary<string, object>>()
                    };
                    byNiss[nissCompany] = comp;
                    companies.Add(comp);
                }
                ((List<Dictionary<string, object>>)comp["contracts"]).Add(ct);
                comp["numContracts"] = (int)comp["numContracts"] + 1;

                var ctMonths = (List<Dictionary<string, object>>)ct["months"];
                comp["totalMonths"] = (int)comp["totalMonths"] + ctMonths.Count;
                var sum = (decimal)comp["totalRemunDeclarada"];
                foreach (var m in ctMonths)
                    if (m["remunDeclarada"] != null) sum += Convert.ToDecimal(m["remunDeclarada"]);
                comp["totalRemunDeclarada"] = sum;
            }

            return Ok(new Dictionary<string, object>
            {
                ["niss"] = worker[0]["NISS"],
                ["nome"] = worker[0]["nome"],
                ["companies"] = companies
            });
        }

        // ---------- Helper: chạy SQL tham số hoá -> list<dict> ----------
        private List<Dictionary<string, object>> Query(string sql, params (string, object)[] ps)
        {
            var result = new List<Dictionary<string, object>>();
            var conn = _ctx.Database.GetDbConnection();
            var opened = false;
            if (conn.State != ConnectionState.Open) { conn.Open(); opened = true; }
            try
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (var (name, val) in ps)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = name;
                        p.Value = val ?? DBNull.Value;
                        cmd.Parameters.Add(p);
                    }
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (var i = 0; i < rd.FieldCount; i++)
                                row[rd.GetName(i)] = rd.IsDBNull(i) ? null : rd.GetValue(i);
                            result.Add(row);
                        }
                    }
                }
            }
            finally
            {
                if (opened) conn.Close();
            }
            return result;
        }
    }
}

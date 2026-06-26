# Benefit Data API — Spec (raw-only)

> **Nguyên tắc:** Contribution = *single source of truth*, chỉ trả **dữ liệu thô**.
> KHÔNG tính média / thâm niên / đủ-điều-kiện / số tiền hưu. Mọi công thức nằm ở **Benefit engine**.
> Mọi mã FK (sexo, estadoCivil, nacionalidade, regime, tipoContrato, profissao, tipoDocumento)
> trả **nguyên giá trị gốc**; Benefit tự map.

## Kết nối

- Base URL: `http://localhost:5000/api/benefit-data` (backend ASP.NET Core 3.1).
- Auth: hiện `[AllowAnonymous]` (bỏ qua lớp JWT/base64 obfuscation) — server-to-server nội bộ.
  **Production phải** giới hạn bằng network/API-key trước khi mở ra ngoài.
- Trả JSON. Ngày dạng ISO `YYYY-MM-DDTHH:mm:ss`. Số thập phân = decimal.

---

## Hàm 1 — Thông tin công ty theo NISS công ty

`GET /api/benefit-data/company/{niss}`

Ví dụ `GET /company/900000003`:
```json
{
  "NISS": "900000003",
  "nome": "Pixel Asia Production Dili Unipessoal, Lda",
  "TIN": "9001152",
  "dataInicioActiv": "2024-01-01T00:00:00",   // ngày bắt đầu hoạt động / thành lập
  "dtInscricao": "2024-01-01T00:00:00",        // ngày đăng ký INSS
  "numTrabalhador": 10,
  "rua": null, "numPorta": null, "aldeiaFk": null,   // địa chỉ chính (MORADA, có thể null)
  "telemovel": null, "email": null                    // liên hệ (CONTACTO, có thể null)
}
```
404 nếu NISS không tồn tại.

---

## Hàm 2 — HỒ SƠ ĐẦY ĐỦ của NLĐ theo NISS NLĐ

`GET /api/benefit-data/worker/{niss}` → master + `documentos[]` + `moradas[]` + `contactos[]` + `inssEstrangeiro[]`.

Ví dụ `GET /worker/100979891`:
```json
{
  "NISS": "100979891", "nome": "ACACIO. LDA",
  "dataNasc": "1992-02-05T00:00:00", "naturalidade": "DILI",
  "sexoFk": 9,           "sexoDesc": "Masculino",  "sexoDescEN": "Male",
  "estadoCivilFk": 13,   "estadoCivilDesc": "Solteiro", "estadoCivilDescEN": "Single",
  "nacionalidadeFk": 11, "nacionalidadeDesc": "Timor-Leste", "nacionalidadeDescEN": "...",
  // ⚠️ DÙNG *Desc, ĐỪNG hardcode mã FK — mã DOMINIO có thể đổi giữa các bản DB
  //    (đã thấy: Viuvo đổi 1110 -> 1111). *Desc luôn đúng.
  "TIN": "12346", "numInscProvisoria": "111111112",
  "nomeMae": "Ikulu Costa", "indDescNomeMae": false,   // indDesc*=true nghĩa là "không rõ tên"
  "nomePai": "Martinho",     "indDescNomePai": false,
  "interno": false,          // true = người của chính INSS

  "documentos": [
    { "idDoc": 5086, "tipoFk": 22, "tipoDesc": "Cartão eleitoral", "tipoDescEN": "Eletroral Card",
      "numero": "1234556", "dataEmissao": null, "dataValidade": "2030-02-01T00:00:00",
      "localEmissao": null, "fileName": "INACIO_PASSPORT.pdf", "hasFile": 1 } ],

  "moradas": [   // địa chỉ + chuỗi địa giới đọc được
    { "idMorada": 5124, "rua": "Manleuana", "numPorta": "45", "moradaPrincipal": false,
      "aldeiaFk": 2273, "aldeia": "Aninfuic", "sucoFk": 448, "suco": "Manleuana",
      "postoFk": 30, "posto": "Dom Aleixo", "municipioFk": 6, "municipio": "Díli",
      "paisFk": 1213, "pais": "Timor-Leste" } ],

  "contactos": [
    { "idContacto": 9464, "telemovel": "73696930", "email": "costaikulu4@gmail.com", "indActivo": true } ],

  "inssEstrangeiro": [   // an sinh nước ngoài (thường rỗng)
    { "id": 1, "nomeSSEstrangeiro": "bpjs", "paisFk": 1101, "pais": "Indonésia",
      "NISSEstrangeiro": "100025649", "indDecontAtualmente": true, "indBenfAtualmente": true,
      "nomeDocumento": "...pdf", "hasFile": 1 } ]
}
```
404 nếu NISS không tồn tại. Mọi mảng có thể rỗng (`[]`). `moradas[].moradaPrincipal=true` = địa chỉ chính.

### Hàm 2b — Tải file PDF giấy tờ

`GET /api/benefit-data/document/{idDoc}` → trả **file PDF nhị phân** (`Content-Type: application/pdf`,
`Content-Disposition: attachment; filename=...`). `idDoc` lấy từ `documentos[].idDoc` của Hàm 2.
404 nếu không có giấy tờ hoặc không có file.

**Loại giấy tờ đi kèm trong response header** (⚠️ tên file KHÔNG đáng tin để suy ra loại —
vd file `INACIO_PASSPORT.pdf` thực ra là *Cartão eleitoral*). Giá trị URL-encoded, client `decodeURIComponent`:
- `X-Document-Type-Fk` — mã loại (vd `22`)
- `X-Document-Type` — mô tả PT, vd `Cart%C3%A3o%20eleitoral` → "Cartão eleitoral"
- `X-Document-Type-EN` — mô tả EN, vd `Eletroral%20Card`
- `X-Document-Number` — số giấy tờ

Các loại đang có file trong DB: **Cartão eleitoral (22), Bilhete de Identidade (1), Passaporte (2)**.

---

## Hàm 3 — Lịch sử đóng góp theo NISS NLĐ (lồng theo từng hợp đồng)

`GET /api/benefit-data/contributions/{niss}`

Cấu trúc **2 tầng: CÔNG TY → các HỢP ĐỒNG → các THÁNG**. Xử lý được cả 2 case:
- **1 công ty ký nhiều hợp đồng** → 1 phần tử `companies[]`, nhiều phần tử `contracts[]` bên trong.
- **NLĐ làm nhiều công ty** → nhiều phần tử `companies[]`.

Mỗi **công ty** có `totalMonths` (số tháng đóng) + `totalRemunDeclarada` (tổng giá trị) — **CHỈ TIỆN ÍCH HIỂN THỊ**, Benefit engine tự tính lại từ `months[]`.
Mỗi **hợp đồng** có 3 mốc: `dtIniVincTrabalhador` (ngày ký HĐ/bắt đầu), `dtIniFimTrabalhador` (ngày kết HĐ, null=đang làm), `lastContribMonth` (tháng đóng cuối = "hết thấy đóng góp").
`months[]` chỉ chứa tháng **có declaração** (status `CONTRIBUTED`); tháng trống = gap → Benefit tự điền & quyết định tính N.
`suspensions[]` = kỳ tạm dừng thô (Benefit tự gắn SUSPENSO).

Ví dụ `GET /contributions/199100002`:
```json
{
  "niss": "199100002",
  "nome": "Jose Faria de Sousa",
  "companies": [
    {
      "nissCompany": "900000003",
      "nomeCompany": "Pixel Asia Production Dili Unipessoal, Lda",
      "numContracts": 1,
      "totalMonths": 13,                 // số tháng công ty này đóng (display)
      "totalRemunDeclarada": 5844.53,    // tổng giá trị (display — engine tính lại)
      "contracts": [
        {
          "idRel": 8062,
          "dtIniVincTrabalhador": "2024-01-01T00:00:00",   // ngày ký HĐ
          "dtIniFimTrabalhador": null,                      // null = đang làm
          "tipoContratoFk": 4,  "tipoContratoDesc": "Por tempo determinado (Lei Trabalho)",
          "regimeFk": 23,       "regimeDesc": "Regime Geral", "regimeDescEN": "General Regime",
          "profissaoFk": 52,    "profissaoDesc": "Outro",
          // ⚠️ dùng *Desc, đừng hardcode mã. LƯU Ý: regimeFk mức HỢP ĐỒNG (tra DOMINIO, vd 23)
          //    KHÁC hệ mã với month.regimeFk mức THÁNG (tra bảng REGIME, vd 1) — cả 2 đều có regimeDesc.
          "firstContribMonth": "2024-02-01T00:00:00",
          "lastContribMonth":  "2025-05-01T00:00:00",
          "months": [
            { "mesAno":"2024-02-01T00:00:00", "remunDeclarada":450.00, "decimoTerceiro":0.00,
              "regimeFk":1, "regimeDesc":"Regime Geral",
              "taxaEntidade":6.0, "taxaTrabalhador":4.0,   // % theo regime (tham chiếu — benefit TỰ tính nếu cần)
              "diasTrabSegSocial":30.0, "diasContrato":30.0, "diasEfecTrabalhados":30.0,
              "faltasInjustific":0, "diasParentalidade":0, "oficioso":false,
              "status":"CONTRIBUTED" }
            // ...
          ],
          "suspensions": [ /* { "dataInicio":"...", "dataFim":"..." } */ ]
        }
      ]
    },
    {
      "nissCompany": "100979899", "nomeCompany": "Hoan test",
      "numContracts": 1, "totalMonths": 2, "totalRemunDeclarada": 344.99,
      "contracts": [ { "idRel": 8072, "dtIniVincTrabalhador":"2025-01-01T00:00:00",
                       "dtIniFimTrabalhador":"2025-12-31T00:00:00", "months":[/*...*/], "suspensions":[] } ]
    }
  ]
}
```
404 nếu NISS không tồn tại. Nếu NLĐ chưa có HĐ nào → `companies: []`.
Ví dụ **1 công ty nhiều HĐ**: `GET /contributions/100006105` → công ty `900000003` có `numContracts:2`.

> **⚠️ NGUYÊN TẮC: chỉ trả số DOANH NGHIỆP ĐÃ KHAI BÁO, KHÔNG tự tính.**
> - `months[]` chỉ gồm tháng thực có khai báo. Vd HĐ 12 tháng nhưng DN chỉ khai 5 tháng → trả đúng 5 tháng,
>   KHÔNG suy diễn/điền 7 tháng còn lại. (NLĐ chưa khai tháng nào → `months: []`.)
> - DN chỉ khai báo **lương** (`remunDeclarada`), KHÔNG khai số tiền đóng → API **không nhân sẵn** tiền đóng.
>   Có gửi kèm `taxaEntidade`/`taxaTrabalhador` (% theo regime, master-data) để benefit TỰ tính nếu muốn
>   (tiền đóng = %×(remunDeclarada+decimoTerceiro)). Số tiền thực đóng per-NLĐ không lưu sẵn trong DB
>   (CONTACORRENTE chỉ gộp theo công ty/tháng).
>
> **Lưu ý raw:** `remunDeclarada` = lương khai báo gốc theo tháng (biến R cho công thức).
> `regimeFk` thô (Benefit map RTSS/RG theo mốc 01/10/2017). `DISPENSACONTRIBUTIVA` là bảng %
> theo năm toàn cục (không gắn NLĐ) → KHÔNG nằm trong hàm này.

---

## Bảng tra mã DOMINIO (cho thiết kế DB Benefit)

| Nhóm | Mã → mô tả |
|---|---|
| **SEXO** | 9=Masculino/Male, 10=Feminino/Female |
| **ESTADOCIVIL** | 13=Solteiro, 14=Casado, 1109=Divorciado, 1110=Viuvo |
| **TIPODOCUMENTO** (giấy tờ tuỳ thân) | 1=Bilhete de Identidade, 2=Passaporte, 21=Certidão, 22=Cartão eleitoral |
| NACIONALIDADE / PROFISSAO / REGIME / TIPOCONTRACTO / NATUREZACONTRACTO / LEILABORALAPLICAVEL | tra trong bảng `DOMINIO` (cột `dominio`,`valor`,`descricao`,`descricaoEN`) |

---

## Bản kê TOÀN BỘ trường gắn với 1 WORKER (để thiết kế DB Benefit)

### TRABALHADOR (master 1-1)
`idTrabalhador` (PK), `nome`, `NISS`, `TIN`, `numInscProvisoria`, `dataNasc`,
`nomeMae`, `indDescNomeMae` (bit – mẹ vô danh), `nomePai`, `indDescNomePai` (bit – cha vô danh),
`estadoCivil` (FK), `naturalidade`, `sexoTrabalhador` (FK), `nacionalidadeTrabalhador` (FK),
`interno` (bit – là người của chính INSS).
*(audit: flagImportado, utilizadorCriacao, dataCriacao, utilizadorAlteracao, dataAlteracao, ipv6)*

### DOCUMENTOIDENTIFICACAO (1-n) — `trabalhador_documeto_fk`
`tpDocIdentificacao` (FK loại), `numero`, `localEmissao`, `dataEmissao`, `dataValidade`,
`documento` (varbinary – **file PDF**), `nomeDocumento`, `indActivo`.

### MORADA (1-n, địa chỉ NLĐ) — `trabalhador_morada_fk`
`rua`, `numPorta`, `morada_aldeia_fk` (FK địa giới), `morada_pais_fk` (FK quốc gia), `moradaPrincipal` (bit).

### CONTACTO (1-n, liên hệ NLĐ) — `contacto_trabalhador_fk`
`telemovel`, `email`, `indActivo`.

### INSSESTRANGEIRO (0-n, an sinh nước ngoài) — `estrangeiro_trabalhador_fk`
`nomeSSEstrangeiro`, `estrangeiro_pais_fk` (FK nước), `NISSEstrangeiro`,
`indDecontAtualmente` (bit – đang đóng ở nước ngoài), `indBenfAtualmente` (bit – đang hưởng),
`documento` (varbinary PDF), `nomeDocumento`.

### RELENTIDADETRABALHADOR (1-n, hợp đồng/việc làm) — `trabalhador_fk`
`idRel` (PK), `entidade_fk` (→ công ty), `tipoContrato` (FK), `naturezaContrato` (FK),
`leiLabAplicavel` (FK), `horasSemana`, `diasSemana`, `dtIniVincTrabalhador` (ngày ký HĐ),
`dtIniFimTrabalhador` (ngày kết HĐ), `funcPublico` (bit), `numFuncPublico`,
`regime_fk` (FK), `escalao_fk` (FK), `profissao` (FK), `profissaoOutro`.

### DECLARACAOREMUNERACAO (1-n, khai báo lương/tháng) — `declaracao_relEntidadeTrabalhador_FK`
`mesAno`, `remunDeclarada`, `decimoTerceiro`, `RegimeFK`, `diasContrato`, `diasEfecTrabalhados`,
`diasTrabcontabSegSocial`, `faltasInjustific`, `diasParentalidade`, `oficioso` (bit), `indActivo`.

### SUSPENSOES (0-n, kỳ tạm dừng) — `trabalhador_suspensao_fk` + `entidade_suspensao_fk`
`dataInicioSuspensao`, `dataFimSuspensao`, `indActivo`.

---

*Backend: `Controllers/BenefitDataController.cs`. SQL đã verify trên DB thật `TimorINSSModuloContribuicoes`.*

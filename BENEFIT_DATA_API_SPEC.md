# Benefit Data API — Spec (raw-only)

> **Principle:** Contribution = *single source of truth*, returns **raw data only**.
> It does NOT compute média / seniority / eligibility / pension amount. All formulas live in the **Benefit engine**.
> Every FK code (sexo, estadoCivil, nacionalidade, regime, tipoContrato, profissao, tipoDocumento)
> is returned **as the original value**; Benefit maps them itself.

## Connection

- Base URL: `http://localhost:5000/api/benefit-data` (ASP.NET Core 3.1 backend).
- Auth: currently `[AllowAnonymous]` (bypasses the JWT/base64 obfuscation layer) — internal server-to-server.
  **In production you MUST** restrict it via network/API-key before exposing it externally.
- Returns JSON. Dates in ISO `YYYY-MM-DDTHH:mm:ss`. Decimal numbers = decimal.

---

## Function 1 — Company info by company NISS

`GET /api/benefit-data/company/{niss}`

Example `GET /company/900000003`:
```json
{
  "NISS": "900000003",
  "nome": "Pixel Asia Production Dili Unipessoal, Lda",
  "TIN": "9001152",
  "dataInicioActiv": "2024-01-01T00:00:00",   // activity start / incorporation date
  "dtInscricao": "2024-01-01T00:00:00",        // INSS registration date
  "numTrabalhador": 10,
  "rua": null, "numPorta": null, "aldeiaFk": null,   // main address (MORADA, may be null)
  "telemovel": null, "email": null                    // contact (CONTACTO, may be null)
}
```
404 if the NISS does not exist.

---

## Function 2 — FULL PROFILE of a worker by worker NISS

`GET /api/benefit-data/worker/{niss}` → master + `documentos[]` + `moradas[]` + `contactos[]` + `inssEstrangeiro[]`.

Example `GET /worker/100979891`:
```json
{
  "NISS": "100979891", "nome": "ACACIO. LDA",
  "dataNasc": "1992-02-05T00:00:00", "naturalidade": "DILI",
  "sexoFk": 9,           "sexoDesc": "Masculino",  "sexoDescEN": "Male",
  "estadoCivilFk": 13,   "estadoCivilDesc": "Solteiro", "estadoCivilDescEN": "Single",
  "nacionalidadeFk": 11, "nacionalidadeDesc": "Timor-Leste", "nacionalidadeDescEN": "...",
  // ⚠️ USE *Desc, DON'T hardcode FK codes — DOMINIO codes can change between DB versions
  //    (observed: Viuvo changed 1110 -> 1111). *Desc is always correct.
  "TIN": "12346", "numInscProvisoria": "111111112",
  "nomeMae": "Ikulu Costa", "indDescNomeMae": false,   // indDesc*=true means "name unknown"
  "nomePai": "Martinho",     "indDescNomePai": false,
  "interno": false,          // true = a person belonging to INSS itself

  "documentos": [
    { "idDoc": 5086, "tipoFk": 22, "tipoDesc": "Cartão eleitoral", "tipoDescEN": "Eletroral Card",
      "numero": "1234556", "dataEmissao": null, "dataValidade": "2030-02-01T00:00:00",
      "localEmissao": null, "fileName": "INACIO_PASSPORT.pdf", "hasFile": 1 } ],

  "moradas": [   // address + human-readable administrative-boundary chain
    { "idMorada": 5124, "rua": "Manleuana", "numPorta": "45", "moradaPrincipal": false,
      "aldeiaFk": 2273, "aldeia": "Aninfuic", "sucoFk": 448, "suco": "Manleuana",
      "postoFk": 30, "posto": "Dom Aleixo", "municipioFk": 6, "municipio": "Díli",
      "paisFk": 1213, "pais": "Timor-Leste" } ],

  "contactos": [
    { "idContacto": 9464, "telemovel": "73696930", "email": "costaikulu4@gmail.com", "indActivo": true } ],

  "inssEstrangeiro": [   // foreign social security (usually empty)
    { "id": 1, "nomeSSEstrangeiro": "bpjs", "paisFk": 1101, "pais": "Indonésia",
      "NISSEstrangeiro": "100025649", "indDecontAtualmente": true, "indBenfAtualmente": true,
      "nomeDocumento": "...pdf", "hasFile": 1 } ]
}
```
404 if the NISS does not exist. Any array may be empty (`[]`). `moradas[].moradaPrincipal=true` = main address.

### Function 2b — Download document PDF file

`GET /api/benefit-data/document/{idDoc}` → returns the **binary PDF file** (`Content-Type: application/pdf`,
`Content-Disposition: attachment; filename=...`). `idDoc` comes from `documentos[].idDoc` in Function 2.
404 if there is no document or no file.

**The document type is carried in the response headers** (⚠️ the file name is NOT reliable for inferring the type —
e.g. file `INACIO_PASSPORT.pdf` is actually a *Cartão eleitoral*). Values are URL-encoded; the client should `decodeURIComponent`:
- `X-Document-Type-Fk` — type code (e.g. `22`)
- `X-Document-Type` — PT description, e.g. `Cart%C3%A3o%20eleitoral` → "Cartão eleitoral"
- `X-Document-Type-EN` — EN description, e.g. `Eletroral%20Card`
- `X-Document-Number` — document number

Types that currently have files in the DB: **Cartão eleitoral (22), Bilhete de Identidade (1), Passaporte (2)**.

---

## Function 3 — Contribution history by worker NISS (nested per contract)

`GET /api/benefit-data/contributions/{niss}`

Structure has **2 levels: COMPANY → CONTRACTS → MONTHS**. Handles both cases:
- **One company signs multiple contracts** → 1 `companies[]` element, multiple `contracts[]` inside.
- **Worker employed at multiple companies** → multiple `companies[]` elements.

Each **company** has `totalMonths` (number of contributed months) + `totalRemunDeclarada` (total value) — **DISPLAY CONVENIENCE ONLY**, the Benefit engine recomputes them from `months[]`.
Each **contract** has 3 milestones: `dtIniVincTrabalhador` (contract sign / start date), `dtIniFimTrabalhador` (contract end date, null = still active), `lastContribMonth` (last contributed month = "no contribution seen after this").
`months[]` contains only months that **have a declaração** (status `CONTRIBUTED`); a missing month = gap → Benefit fills it in and decides how to compute N.
`suspensions[]` = raw suspension periods (Benefit attaches SUSPENSO itself).

Example `GET /contributions/199100002`:
```json
{
  "niss": "199100002",
  "nome": "Jose Faria de Sousa",
  "companies": [
    {
      "nissCompany": "900000003",
      "nomeCompany": "Pixel Asia Production Dili Unipessoal, Lda",
      "numContracts": 1,
      "totalMonths": 13,                 // months this company contributed (display)
      "totalRemunDeclarada": 5844.53,    // total value (display — engine recomputes)
      "contracts": [
        {
          "idRel": 8062,
          "dtIniVincTrabalhador": "2024-01-01T00:00:00",   // contract sign date
          "dtIniFimTrabalhador": null,                      // null = still active
          "tipoContratoFk": 4,  "tipoContratoDesc": "Por tempo determinado (Lei Trabalho)",
          "regimeFk": 23,       "regimeDesc": "Regime Geral", "regimeDescEN": "General Regime",
          "profissaoFk": 52,    "profissaoDesc": "Outro",
          // ⚠️ use *Desc, don't hardcode codes. NOTE: contract-level regimeFk (look up in DOMINIO, e.g. 23)
          //    uses a DIFFERENT code system than month-level month.regimeFk (look up in REGIME table, e.g. 1) — both carry regimeDesc.
          "firstContribMonth": "2024-02-01T00:00:00",
          "lastContribMonth":  "2025-05-01T00:00:00",
          "months": [
            { "mesAno":"2024-02-01T00:00:00", "remunDeclarada":450.00, "decimoTerceiro":0.00,
              "regimeFk":1, "regimeDesc":"Regime Geral",
              "taxaEntidade":6.0, "taxaTrabalhador":4.0,   // % per regime (reference — benefit computes itself if needed)
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
404 if the NISS does not exist. If the worker has no contracts → `companies: []`.
Example of **one company, multiple contracts**: `GET /contributions/100006105` → company `900000003` has `numContracts:2`.

> **⚠️ PRINCIPLE: return only what the EMPLOYER DECLARED, do NOT infer.**
> - `months[]` contains only months that were actually declared. E.g. a 12-month contract but the employer declared only 5 months → return exactly 5 months,
>   do NOT infer/fill the remaining 7. (Worker who declared no month → `months: []`.)
> - The employer declares only the **salary** (`remunDeclarada`), NOT the contribution amount → the API does **not** pre-multiply contributions.
>   It does include `taxaEntidade`/`taxaTrabalhador` (% per regime, master-data) so benefit can compute it itself if desired
>   (contribution = %×(remunDeclarada+decimoTerceiro)). The actual per-worker contribution amount is not stored in the DB
>   (CONTACORRENTE aggregates only by company/month).
>
> **Raw note:** `remunDeclarada` = original declared monthly salary (variable R for the formula).
> `regimeFk` is raw (Benefit maps RTSS/RG by the 2017-10-01 cutoff). `DISPENSACONTRIBUTIVA` is a global per-year %
> table (not tied to a worker) → NOT part of this function.

---

## DOMINIO code lookup table (for Benefit DB design)

| Group | Code → description |
|---|---|
| **SEXO** | 9=Masculino/Male, 10=Feminino/Female |
| **ESTADOCIVIL** | 13=Solteiro, 14=Casado, 1109=Divorciado, 1110=Viuvo |
| **TIPODOCUMENTO** (identity documents) | 1=Bilhete de Identidade, 2=Passaporte, 21=Certidão, 22=Cartão eleitoral |
| NACIONALIDADE / PROFISSAO / REGIME / TIPOCONTRACTO / NATUREZACONTRACTO / LEILABORALAPLICAVEL | look up in the `DOMINIO` table (columns `dominio`,`valor`,`descricao`,`descricaoEN`) |

---

## Full inventory of fields tied to ONE WORKER (for Benefit DB design)

### TRABALHADOR (master 1-1)
`idTrabalhador` (PK), `nome`, `NISS`, `TIN`, `numInscProvisoria`, `dataNasc`,
`nomeMae`, `indDescNomeMae` (bit – mother unknown), `nomePai`, `indDescNomePai` (bit – father unknown),
`estadoCivil` (FK), `naturalidade`, `sexoTrabalhador` (FK), `nacionalidadeTrabalhador` (FK),
`interno` (bit – belongs to INSS itself).
*(audit: flagImportado, utilizadorCriacao, dataCriacao, utilizadorAlteracao, dataAlteracao, ipv6)*

### DOCUMENTOIDENTIFICACAO (1-n) — `trabalhador_documeto_fk`
`tpDocIdentificacao` (FK type), `numero`, `localEmissao`, `dataEmissao`, `dataValidade`,
`documento` (varbinary – **PDF file**), `nomeDocumento`, `indActivo`.

### MORADA (1-n, worker address) — `trabalhador_morada_fk`
`rua`, `numPorta`, `morada_aldeia_fk` (FK administrative boundary), `morada_pais_fk` (FK country), `moradaPrincipal` (bit).

### CONTACTO (1-n, worker contact) — `contacto_trabalhador_fk`
`telemovel`, `email`, `indActivo`.

### INSSESTRANGEIRO (0-n, foreign social security) — `estrangeiro_trabalhador_fk`
`nomeSSEstrangeiro`, `estrangeiro_pais_fk` (FK country), `NISSEstrangeiro`,
`indDecontAtualmente` (bit – currently contributing abroad), `indBenfAtualmente` (bit – currently receiving benefit),
`documento` (varbinary PDF), `nomeDocumento`.

### RELENTIDADETRABALHADOR (1-n, contract/employment) — `trabalhador_fk`
`idRel` (PK), `entidade_fk` (→ company), `tipoContrato` (FK), `naturezaContrato` (FK),
`leiLabAplicavel` (FK), `horasSemana`, `diasSemana`, `dtIniVincTrabalhador` (contract sign date),
`dtIniFimTrabalhador` (contract end date), `funcPublico` (bit), `numFuncPublico`,
`regime_fk` (FK), `escalao_fk` (FK), `profissao` (FK), `profissaoOutro`.

### DECLARACAOREMUNERACAO (1-n, salary declaration per month) — `declaracao_relEntidadeTrabalhador_FK`
`mesAno`, `remunDeclarada`, `decimoTerceiro`, `RegimeFK`, `diasContrato`, `diasEfecTrabalhados`,
`diasTrabcontabSegSocial`, `faltasInjustific`, `diasParentalidade`, `oficioso` (bit), `indActivo`.

### SUSPENSOES (0-n, suspension periods) — `trabalhador_suspensao_fk` + `entidade_suspensao_fk`
`dataInicioSuspensao`, `dataFimSuspensao`, `indActivo`.

---

*Backend: `Controllers/BenefitDataController.cs`. SQL verified against the real DB `TimorINSSModuloContribuicoes`.*

# Thay đổi cơ sở dữ liệu — Módulo Contabilidade (mode mới) / Database Changes — Módulo Contabilidade (new mode)

*Cập nhật / Last updated: 2026-07-13*

## 1. Tổng quan / Overview

**VI** — Tài liệu này liệt kê **toàn bộ** thay đổi trên cơ sở dữ liệu kể từ khi bắt đầu xây dựng "mode mới" (Módulo Contabilidade), so với bản backup gốc chụp **ngay trước khi bắt đầu** (`TimorINSSModuloContribuicoes_2026-07-11_pre_m1.bak`, 2026-07-11 00:32). Danh sách này được xác minh bằng cách restore bản backup gốc vào một DB tạm và **so sánh trực tiếp schema** (INFORMATION_SCHEMA) với DB hiện tại — không chỉ dựa vào việc đọc lại các file script, để đảm bảo không bỏ sót thay đổi nào (kể cả những thay đổi có thể đã áp trực tiếp mà quên lưu script).

**EN** — This document lists **all** database changes made since the start of the "new mode" (Módulo Contabilidade) build, compared against the original backup taken **right before work began** (`TimorINSSModuloContribuicoes_2026-07-11_pre_m1.bak`, 2026-07-11 00:32). The list was verified by restoring that original backup into a scratch database and directly **diffing the schema** (INFORMATION_SCHEMA) against the current database — not just re-reading the migration scripts — to catch any change that may have been applied directly and not saved as a script.

**Kết quả xác minh / Verification result (re-verified 2026-07-13):**

| | Số lượng / Count |
|---|---|
| Bảng mới / New tables | **31** |
| Cột mới trên bảng đã có sẵn / New columns on pre-existing tables | **1** |
| Bảng bị xóa / Tables removed | **0** |
| Cột bị xóa / Columns removed | **0** |
| Kiểu dữ liệu/cột bị đổi trên bảng cũ / Changed columns on pre-existing tables | **0** |

**VI** — Tức là gần như toàn bộ thay đổi mang tính **cộng thêm (additive)**: không có bảng nào bị xóa, không có cột nào bị xóa hay đổi kiểu trên các bảng đã tồn tại trước đó — chỉ có **1 cột mới** được thêm vào 1 bảng cũ (`ContaBancaria.CodigoContaFk`, xem mục 3). Dữ liệu và các màn hình cũ (mode cũ) không bị ảnh hưởng. Khách hàng chỉ cần chạy **1 script SQL duy nhất đã gộp sẵn** (mục 6, Phần 2) lên DB của mình — không cần lo về việc mất dữ liệu.

**EN** — In other words, nearly every change is strictly **additive**: no table was dropped, and no column on a pre-existing table was removed or had its type changed — only **1 new column** was added to 1 pre-existing table (`ContaBancaria.CodigoContaFk`, see section 3). Old-mode data and screens are unaffected. The client only needs to run **one single, already-bundled SQL script** (section 6, Part 2) against their database — there is no data-loss risk.

Toàn bộ script SQL gốc, theo đúng thứ tự đã chạy, nằm ở thư mục `db_migrations/` trong repo (35 file, đặt tên theo ngày, 2026-07-11 → 2026-07-13). **Phần 2 (mục 6) đã có sẵn 1 file gộp** (`Guideline/CONSOLIDATED_client_handoff_2026-07-13.sql`) — đã được kiểm thử thật: chạy trên 1 bản copy sạch của baseline gốc rồi so sánh schema kết quả với DB hiện tại, khớp 100% (bảng/cột/kiểu dữ liệu).
All original SQL scripts, in the exact order they were run, are in the `db_migrations/` folder in the repo (35 files, dated, 2026-07-11 → 2026-07-13). **Part 2 (section 6) now exists as one bundled file** (`Guideline/CONSOLIDATED_client_handoff_2026-07-13.sql`) — real-tested: run against a clean copy of the original baseline, then the resulting schema was diffed against the current database and matched 100% (tables/columns/data types).

---

## 2. Bảng mới, theo nhóm nghiệp vụ / New tables, by functional area

### 2.1 Dữ liệu gốc (Master data) / Master data

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `BudgetPeriod` | Kỳ ngân sách (năm + loại Principal/Suplementar) dùng riêng cho mode mới. Bảng **mới, tách riêng** khỏi `Orcamentoconfig` cũ (mode cũ vẫn dùng `Orcamentoconfig` cho `Codigoconta`/`CentroCusto`/sổ đăng ký ngân sách cũ, không đổi) — xem mục 3. Mọi bảng mode mới bên dưới neo vào đây qua `BudgetPeriodFk`. | Budget period (year + Principal/Suplementar type) used exclusively by the new mode. A **new, dedicated** table — split off from the pre-existing `Orcamentoconfig` (old mode still uses `Orcamentoconfig` for `Codigoconta`/`CentroCusto`/the old budget register, unchanged) — see section 3. Every new-mode table below anchors to this one via `BudgetPeriodFk`. |
| `ProgramActivity` | Cây Programa → Subprograma → Atividade, có phiên bản theo năm ngân sách (`BudgetPeriodFk`). Có cờ `IsOssPerimeter` đánh dấu chương trình có nằm trong phạm vi OSS hay không (A07 trong perimeter, A08 ngoài perimeter). | Programa → Subprograma → Atividade tree, versioned per budget year (`BudgetPeriodFk`). Includes an `IsOssPerimeter` flag marking whether the program falls inside the OSS consolidation scope (A07 in-perimeter, A08 out-of-perimeter). |
| `FunctionalClassification` | Classificação Funcional kiểu COFOG, 2 cấp, không đổi theo năm. | COFOG-style Classificação Funcional, 2 levels, evergreen (not year-versioned). |
| `EconomicClassification` | Classificação Económica (mã ngân sách Receita 4xx / Despesa 5xx), versioned theo `BudgetPeriodFk`. Bảng **mới, tách riêng** khỏi `Codigoconta` hiện có — `Codigoconta` là hệ thống tài khoản kế toán (SNC-TL, Débito/Crédito), khác với phân loại kinh tế ngân sách dù mã số đầu trông giống nhau. | Classificação Económica (Receita 4xx / Despesa 5xx budget codes), versioned via `BudgetPeriodFk`. A **new, dedicated** table — deliberately not merged with the existing `Codigoconta`, which is the SNC-TL general-ledger chart of accounts (Débito/Crédito), a different taxonomy despite similar leading digits. |

### 2.2 Ngân sách — Orçamento / Budget — Orçamento

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `OrcamentoBatch` | Lô ngân sách theo năm, quy trình duyệt DRAFT → PENDING_REVIEW → PENDING_APPROVAL → APPROVED, duyệt theo **cả lô**. | Yearly budget batch; DRAFT → PENDING_REVIEW → PENDING_APPROVAL → APPROVED approval workflow, approved as **one whole batch**. |
| `OrcamentoLinha` | Từng dòng rúbrica (Atividade × Classificação Económica × Organization × Ano) với giá trị dự toán, thuộc về 1 `OrcamentoBatch`. | Each rúbrica line (Atividade × Classificação Económica × Organization × Ano) with a budgeted value, belonging to one `OrcamentoBatch`. |
| `OrcamentoSuplementar` | Lô điều chỉnh ngân sách bổ sung (giữa năm), cùng quy trình duyệt 4 bước như Orçamento chính; không giới hạn số lần điều chỉnh trong 1 năm. | Mid-year supplementary budget adjustment batch, same 4-step approval workflow as the main Orçamento; a year may have any number of Suplementar rounds. |
| `OrcamentoSuplementarLinha` | Từng dòng điều chỉnh (giá trị cũ / mức điều chỉnh / giá trị mới) tham chiếu tới 1 `OrcamentoLinha` đã APPROVED. | Each adjustment line (old value / adjustment amount / final value) referencing an already-APPROVED `OrcamentoLinha`. |

### 2.3 Chu trình chi tiêu — AD / Cabimento / Compromisso / Obrigação / Pagamento / Expenditure cycle

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `ExpenditureAuthorization` | AD (Autorização de Despesa) — sinh ra từ 1 `OrcamentoLinha` đã duyệt, quy trình duyệt riêng DRAFT → PENDING_REVIEW → PENDING_APPROVAL → APPROVED. | AD (Autorização de Despesa) — generated from an approved `OrcamentoLinha`, with its own DRAFT → PENDING_REVIEW → PENDING_APPROVAL → APPROVED workflow. |
| `ExpenditureAuthorizationPlurianualidade` | Chi tiết phân bổ AD theo nhiều năm (Plurianualidade), khi 1 AD chi tiêu trải dài hơn 1 năm ngân sách. | Multi-year (Plurianualidade) breakdown for an AD that spans more than one budget year. |
| `Cabimento` | DIC/Cabimento — tách biệt về pháp lý với AD (theo Lei 2/2022, DL 23/2022), sinh ra từ 1 AD đã duyệt, quy trình duyệt ngắn hơn (chỉ tới Diretor DF, không qua DE). | DIC/Cabimento — legally separate from AD (per Lei 2/2022, DL 23/2022), generated from an approved AD, with a shorter workflow (ends at Diretor DF, no DE step). |
| `CompromissoDespesa` | Compromisso mode mới — **khác** bảng `COMPROMISSO` cũ (rỗng, gắn với quy trình tarefaAtivo cũ). Tham chiếu tới 1 Cabimento đã duyệt; 1 Cabimento có thể có nhiều Compromisso. Quy trình duyệt 3 bước (Diretor DF review + DE approve). | New-mode Compromisso — **distinct** from the old (empty) `COMPROMISSO` table tied to the legacy tarefaAtivo workflow. References an approved Cabimento; one Cabimento may have many Compromisso rows. 3-step approval (Diretor DF review + DE approve). |
| `CompromissoDespesaPlurianualidade` | Chi tiết phân bổ Compromisso theo nhiều năm. | Multi-year breakdown for a Compromisso. |
| `Obligation` | Obrigação — gom nhiều Compromisso vào 1 đợt thanh toán chung (quan hệ nhiều-nhiều qua `ObligationItem`). Duyệt 2 bước (chỉ Diretor DF "Aprovação e Liquidação"). | Obrigação — groups multiple Compromisso rows into one combined payment batch (many-to-many via `ObligationItem`). 2-step approval (Diretor DF "Aprovação e Liquidação" only). |
| `ObligationItem` | Bảng liên kết nhiều-nhiều giữa `Obligation` và `CompromissoDespesa`. | Many-to-many link table between `Obligation` and `CompromissoDespesa`. |
| `ObligationBeneficiary` | Danh sách người thụ hưởng/thông tin ngân hàng khi Obrigação là dạng trả nhiều người (trợ cấp/lương) — dùng chung cho cả 2 dạng danh sách trong form gốc (ListaObrigação1: Beneficiários, ListaObrigação2: Pessoal/lương). | Beneficiary/bank-detail list for multi-payee Obrigação (subsidies/payroll) — unifies both list types from the original form (ListaObrigação1: Beneficiários, ListaObrigação2: Pessoal/payroll). |
| `PaymentAuthorization` | Autorização do Pagamento — bước duyệt (Diretor DF), sinh từ 1 Obrigação đã duyệt. Đây là bước **duy nhất** trong chu trình chi tiêu thực sự ghi bút toán Débito/Crédito (tài khoản kế toán `Codigoconta`). | Autorização do Pagamento — the approval step (Diretor DF), generated from an approved Obligation. This is the **only** step in the expenditure chain that books an actual Débito/Crédito GL entry (`Codigoconta`). |
| `PaymentExecution` | Realização do Pagamento — hành động xác nhận đã chi tiền thật, chỉ thực hiện được sau khi `PaymentAuthorization` đã APPROVED. Không có trạng thái riêng — sự tồn tại của dòng này (IndActivo=1) nghĩa là "đã thanh toán". | Realização do Pagamento — the actual money-out confirmation, only allowed once its `PaymentAuthorization` is APPROVED. No separate status column — the row's existence (IndActivo=1) means "paid". |

### 2.4 Thu (Receita) / Revenue (Receita)

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `ReceitaPac` | Ghi nhận Thu mode mới (sheet RECEITAS_PAC — lãi ngân hàng, số dư chuyển kỳ mã 408, v.v.). **Không** bao gồm Thu từ đóng góp BHXH (RECEITAS_GP, mã 401.x) — dữ liệu đó vẫn nằm ở Módulo Contribuições, Finance chỉ đối chiếu/báo cáo. Không có bước duyệt (execution-only). | New-mode Receita entry (RECEITAS_PAC sheet — bank interest, opening-balance carryover code 408, etc). Does **not** cover contribution revenue (RECEITAS_GP, code 401.x) — that data still lives in the Contribuições module; Finance only reconciles/reports it. No approval gate (execution-only). |

### 2.5 Đối chiếu ngân hàng / Bank reconciliation

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `BankStatementLine` | Dòng sao kê ngân hàng mode mới (nhập/import thủ công, không có kết nối ngân hàng tự động), dùng để đối chiếu sau khi Thu/Chi đã ghi nhận. Bảng **mới hoàn toàn**, không tái dùng `MOVIMENTOSBANCARIOS`/`MOVIMENTOSPORCONCILIAR` cũ (gắn với các chiều dữ liệu cũ không còn áp dụng). | New-mode bank statement line (manually entered/imported — no automatic bank feed), used to reconcile after Receita/Despesa are booked. A **fresh** table, not a reuse of the old `MOVIMENTOSBANCARIOS`/`MOVIMENTOSPORCONCILIAR` pair (tied to legacy dimensions no longer applicable). |

### 2.6 Sổ nhật ký kế toán / Accounting journal

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `Lancamento` | Bút toán Débito/Crédito, **tự động sinh ra** tại thời điểm Pagamento thực hiện hoặc Receita được xác nhận — không phải màn hình nhập tay riêng. `OrigemTipo`/`OrigemId` là con trỏ mềm (không phải FK) trỏ về bản ghi nguồn, để bảng này dùng chung được cho nhiều loại nguồn gốc trong tương lai. | Débito/Crédito journal entry, **auto-generated** the moment a Pagamento executes or a Receita is confirmed — not a manually-typed separate journal screen. `OrigemTipo`/`OrigemId` is a soft (non-FK) pointer back to the source record, so the table stays generic across future source types. |
| `GuiaPagamentoContaConfig` | Cấu hình 1 dòng (singleton) chọn sẵn tài khoản Có (tách riêng Setor Público/Privado) dùng để tự sinh `Lancamento` khi 1 Guia Pagamento (GP) được đối chiếu ngân hàng thành công (màn "Duyệt Guia Pagamento — đối chiếu ngân hàng"). Trước đó sự kiện này không sinh bút toán nào. Tài khoản Nợ **không** cấu hình cố định ở đây nữa — được suy ra động theo `ContaBancaria.CodigoContaFk` của chính dòng sao kê ngân hàng đã khớp (xem mục 3). Do admin chọn 1 lần qua màn Cấu hình hệ thống. | A 1-row (singleton) config picking the Crédito account (split Setor Público/Privado) used to auto-generate a `Lancamento` when a Guia Pagamento (GP) is successfully bank-reconciled (the "Duyệt Guia Pagamento — bank reconciliation" screen). Previously that event produced no journal entry at all. The Débito account is **no longer** fixed here — it's resolved dynamically from the matched bank line's own `ContaBancaria.CodigoContaFk` (see section 3). Set once by an admin via System Settings. |
| `LiquidacaoContaConfig` | Cấu hình tài khoản "Phải trả" trung gian dùng cho chu trình Chi tiêu (Autorização/Realização de Pagamento), 1 dòng cho mỗi `Categoria` người thụ hưởng (Fornecedor/Beneficiário/Contribuinte EE/Pessoal/Outro). Xác nhận khớp với sổ sách thật của khách hàng: mọi Despesa đi qua 1 tài khoản Phải trả trung gian, không Nợ thẳng Despesa/Có thẳng Ngân hàng trong 1 bút toán. Seed sẵn 5 dòng theo Categoria, tài khoản để trống (`NULL`) — admin điền qua Cấu hình hệ thống. | Config for the intermediate "Payable" account used by the expenditure cycle (Autorização/Realização de Pagamento), one row per beneficiary `Categoria` (Fornecedor/Beneficiário/Contribuinte EE/Pessoal/Outro). Confirmed against the client's own real ledger: every Despesa passes through an intermediate Payable account, never a direct Despesa-Debit/Bank-Credit single entry. Seeded with 5 rows by Categoria, account left `NULL` — filled in by an admin via System Settings. |

### 2.7 Người dùng & phân quyền / Users & permissions

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `UserModeAccess` | Cơ chế tạm thời quyết định user nào được vào mode mới (có dòng ACTIVE = có quyền vào). Đã được thay thế dần bởi `UserPermission` chi tiết hơn — không mở rộng thêm cột, sẽ bị thay hẳn. | Interim gate deciding which users can enter the new mode (an ACTIVE row = access granted). Being superseded by the more granular `UserPermission` — not extended further, will eventually be fully replaced. |
| `UserPermission` | Gán token quyền (ORC_SUBMIT, AD_APPROVE, ...) cho từng user, cho màn "Quản lý User & Phân quyền" mode mới. Hoàn toàn tách biệt khỏi hệ Perfil/Funcionalidade cũ (mode cũ vẫn dùng nguyên hệ cũ, không đổi). | Assigns permission tokens (ORC_SUBMIT, AD_APPROVE, ...) to individual users, for the new-mode "Quản lý User & Phân quyền" screen. Fully separate from the old Perfil/Funcionalidade system (old mode keeps using its own system unchanged). |
| `PermissionPreset` | Gói quyền định sẵn (ví dụ "Diretor DF") để gán nhanh nhiều token cùng lúc cho 1 user. | A named bundle of permission tokens (e.g. "Diretor DF") for quickly granting many tokens to a user at once. |
| `PermissionPresetItem` | Danh sách token thuộc về 1 `PermissionPreset`. | The list of tokens belonging to one `PermissionPreset`. |
| `UserProfile` | Thông tin bổ sung cho user mode mới (Tên/Email/Phòng ban) — bảng vệ tinh mới, không sửa trực tiếp bảng `Utilizador` cũ. | Extra profile fields for new-mode users (Name/Email/Department) — a new satellite table, does not alter the existing `Utilizador` table directly. |

### 2.8 Cấu hình hệ thống / System configuration

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `LanguageConfig` | Bật/tắt các ngôn ngữ giao diện hiển thị cho người dùng (EN/PT/TET/VI) từ màn admin, thay cho danh sách cố định trong code. | Admin-configurable on/off switch for which UI languages are offered to users (EN/PT/TET/VI), replacing a hardcoded list in code. |
| `AttachmentConfig` | Cấu hình 1 dòng (singleton): dung lượng file tối đa (mặc định 10MB) + cờ bắt buộc đính kèm cho từng loại hồ sơ (AD/Cabimento/Compromisso/Obrigação/Pagamento). Seed sẵn 1 dòng mặc định, tất cả cờ bắt buộc = tắt (không chặn hồ sơ đang xử lý). | 1-row (singleton) config: max file size (default 10MB) + a per-document-type (AD/Cabimento/Compromisso/Obrigação/Pagamento) mandatory-attachment flag. Seeded with 1 default row, every mandatory flag off (doesn't block in-flight records). |

### 2.9 Tệp đính kèm / Attachments

| Bảng / Table | Mục đích (VI) | Purpose (EN) |
|---|---|---|
| `Attachment` | File đính kèm (PDF/ảnh chứng từ...) cho các hồ sơ chu trình Chi tiêu, gắn qua con trỏ mềm `EntityType`/`EntityId` (giống cơ chế `OrigemTipo`/`OrigemId` của `Lancamento`) để dùng chung cho nhiều loại hồ sơ. Kiểm soát bắt buộc/không bắt buộc theo `AttachmentConfig`. | Attached files (PDF/scanned proof documents...) for expenditure-cycle records, linked via a soft pointer `EntityType`/`EntityId` (same pattern as `Lancamento`'s `OrigemTipo`/`OrigemId`) so the table is reusable across record types. Mandatory/optional is governed by `AttachmentConfig`. |

---

## 3. Cột mới trên bảng đã có sẵn / New columns on pre-existing tables

**VI** — **1 cột.** `ContaBancaria.CodigoContaFk` (`int`, cho phép NULL, FK tới `Codigoconta.Id`) — ánh xạ mỗi tài khoản ngân hàng (`ContaBancaria`, bảng cũ đã có sẵn, dùng chung cho cả mode cũ và mới) sang đúng dòng trong hệ thống tài khoản kế toán (`Codigoconta`). Lý do: khi 1 Guia Pagamento (GP) được đối chiếu ngân hàng thành công, bút toán Nợ phải ghi đúng vào tài khoản ngân hàng **cụ thể** đã nhận tiền (đối chiếu với sổ sách thật của khách: mỗi ngân hàng có 1 mã tài khoản kế toán riêng, không dùng chung 1 tài khoản Nợ cố định cho mọi Guia). Cột này để `NULL` cho tới khi admin ánh xạ — Guia đối chiếu bình thường ngay cả khi chưa ánh xạ, chỉ là bút toán tương ứng sẽ tạm chưa được tự sinh cho tới khi ánh xạ xong (hệ thống sẽ báo rõ khi thiếu, không im lặng bỏ qua).

Ngoài ra, mọi nhu cầu "năm ngân sách" khác của mode mới được đưa vào bảng mới `BudgetPeriod` (xem mục 2.1) thay vì chỉnh sửa bảng `Orcamentoconfig` sẵn có — vì `Orcamentoconfig` vẫn đang là mốc năm dùng chung cho dữ liệu kế toán mode cũ (`Codigoconta`, `CentroCusto`, sổ đăng ký ngân sách cũ). `Orcamentoconfig` giữ nguyên 100% cấu trúc gốc.

**EN** — **1 column.** `ContaBancaria.CodigoContaFk` (`int`, nullable, FK to `Codigoconta.Id`) — maps each bank account (`ContaBancaria`, a pre-existing table shared by old and new mode) to its correct chart-of-accounts row (`Codigoconta`). Reason: when a Guia Pagamento (GP) is successfully bank-reconciled, the Débito leg of the journal entry must post to the **specific** bank account that actually received the money (confirmed against the client's real ledger — each bank has its own account code, not one fixed Débito account for every Guia). Left `NULL` until an admin maps it — a Guia can still be reconciled with it unmapped, the corresponding journal entry simply won't auto-generate until it's mapped (the system now surfaces this clearly instead of silently skipping it).

Every other new-mode need for a "budget year" is served by the new `BudgetPeriod` table (see section 2.1) instead of modifying the pre-existing `Orcamentoconfig` table — `Orcamentoconfig` is still the shared year-anchor for old-mode's accounting data (`Codigoconta`, `CentroCusto`, the old budget register). `Orcamentoconfig` keeps its exact original structure, unchanged.

---

## 4. Không có gì bị xóa hoặc đổi / Nothing removed or changed

**VI** — So với baseline, không có bảng nào bị xóa, không có cột nào trên các bảng cũ bị xóa hoặc đổi kiểu dữ liệu/nullable. Mọi thay đổi đều là tạo mới (bảng hoặc cột), an toàn để áp dụng lên DB đang có dữ liệu thật của khách hàng mà không sợ mất hay hỏng dữ liệu hiện có.

**EN** — Compared to the baseline, no table was dropped, and no column on a pre-existing table was removed or had its data type/nullability changed. Every change is a pure addition (new table or new column), safe to apply to the client's live database without risk to existing data.

---

## 5. Dữ liệu khởi tạo cần thiết / Required baseline seed data

**VI** — Ngoài schema (bảng/cột), một số bảng cấu hình cần **sẵn vài dòng dữ liệu cố định** để màn hình liên quan hoạt động đúng ngay từ đầu (không phải dữ liệu nghiệp vụ của khách hàng — là danh mục cố định của ứng dụng). Script gộp ở mục 6 đã tự chèn sẵn các dòng này:

| Bảng | Dữ liệu khởi tạo | Vì sao cần |
|---|---|---|
| `LanguageConfig` | 4 dòng: EN/PT/TET/VI, đều bật | Màn chọn ngôn ngữ cần có dữ liệu để hiển thị |
| `PermissionPreset` + `PermissionPresetItem` | 5 gói quyền mẫu (ADMIN/TECNICO_DF/DIRETOR_DF/DIRETOR_EXECUTIVO/REPORT_ONLY), 27 dòng token | Gói khởi điểm cho màn "Quản lý User & Phân quyền" — có thể sửa/xóa/thêm sau |
| `Perfil` | 1 dòng "Conta Módulo Contabilidade (sem Perfil legado)" | Tài khoản mode mới bắt buộc phải có 1 Perfil để đăng nhập được (giới hạn kỹ thuật của hệ đăng nhập cũ) — dòng này không gắn quyền nào của mode cũ |
| `AttachmentConfig` | 1 dòng mặc định (giới hạn 10MB, chưa bắt buộc đính kèm loại nào) | Bảng cấu hình dạng singleton, cần đúng 1 dòng để không lỗi |
| `LiquidacaoContaConfig` | 5 dòng theo Categoria, tài khoản để trống | Danh mục Categoria cố định; admin điền tài khoản sau |
| `ProgramActivity` (A07/A08 và các nhánh con) | 7 dòng — cấu trúc chương trình "Administração do FRSS"/"Regime Contributivo de Capitalização" theo `OSS_Global_2026_FINAL_livro.xlsx` | Dữ liệu tổ chức ngân sách thật của khách hàng, không phải dữ liệu test — script tự gắn vào kỳ ngân sách (`ORCAMENTOCONFIG`) hiện có của khách khi chạy |
| `BudgetPeriod` | Sao chép động từ đúng các dòng `ORCAMENTOCONFIG` hiện có của khách | Không dùng số liệu cứng từ máy dev — mỗi khách có kỳ ngân sách khác nhau |

**Không** seed sẵn (khách tự cấu hình qua màn hình sau khi chạy script, vì đây là dữ liệu riêng của từng khách): tài khoản ngân hàng thật (`ContaBancaria`), ánh xạ ngân hàng ↔ tài khoản kế toán (`ContaBancaria.CodigoContaFk`), tài khoản Nợ/Có cho `GuiaPagamentoContaConfig`/`LiquidacaoContaConfig`, cấp quyền `UserModeAccess`/`UserPermission` cho người dùng thật.

**EN** — Besides schema (tables/columns), a few config tables need **some fixed rows pre-populated** for their screens to work correctly from day one (not customer business data — a fixed application catalog). The bundled script in section 6 inserts these automatically:

| Table | Seed data | Why it's needed |
|---|---|---|
| `LanguageConfig` | 4 rows: EN/PT/TET/VI, all enabled | The language picker needs rows to show |
| `PermissionPreset` + `PermissionPresetItem` | 5 starter role bundles (ADMIN/TECNICO_DF/DIRETOR_DF/DIRETOR_EXECUTIVO/REPORT_ONLY), 27 token rows | Starting point for the "User & Permission Management" screen — editable/removable later |
| `Perfil` | 1 row "Conta Módulo Contabilidade (sem Perfil legado)" | New-mode accounts must have a Perfil to be able to log in at all (a legacy login-system limitation) — this row carries no old-mode permissions |
| `AttachmentConfig` | 1 default row (10MB limit, nothing mandatory yet) | Singleton config table, needs exactly 1 row to avoid errors |
| `LiquidacaoContaConfig` | 5 rows by Categoria, account left blank | Fixed Categoria catalog; admin fills in the account later |
| `ProgramActivity` (A07/A08 and children) | 7 rows — the "Administração do FRSS"/"Regime Contributivo de Capitalização" program structure per `OSS_Global_2026_FINAL_livro.xlsx` | Real client budget-organization data, not test data — the script links it to the client's existing budget-year config (`ORCAMENTOCONFIG`) at run time |
| `BudgetPeriod` | Dynamically copied from the client's own existing `ORCAMENTOCONFIG` rows | No hardcoded dev-machine values — every client has a different budget period |

**Not** seeded (client configures these via the UI after running the script, since this is client-specific data): real bank accounts (`ContaBancaria`), bank-to-chart-of-accounts mapping (`ContaBancaria.CodigoContaFk`), Débito/Crédito accounts for `GuiaPagamentoContaConfig`/`LiquidacaoContaConfig`, `UserModeAccess`/`UserPermission` grants for real users.

---

## 6. Phần 2 — Script SQL gộp sẵn / Part 2 — Bundled runnable script

**VI** — File **`Guideline/CONSOLIDATED_client_handoff_2026-07-13.sql`** là 1 script SQL duy nhất, gộp lại từ toàn bộ 35 file gốc, đã được rà soát và bỏ đi mọi dữ liệu chỉ có ý nghĩa trên máy phát triển (vd tài khoản ngân hàng giả để test, 1 dòng kỳ ngân sách test năm 2099, cấp quyền thử cho 1 tài khoản test cụ thể) — chỉ giữ lại phần schema thật và dữ liệu khởi tạo cần thiết (mục 5). Đã kiểm thử thật: chạy script này trên 1 bản copy sạch của baseline gốc, sau đó so sánh trực tiếp schema kết quả với DB hiện tại đang chạy — khớp 100% (không thiếu, không thừa bảng/cột/kiểu dữ liệu nào).

Cách chạy:
1. **Backup DB trước** (script chỉ cộng thêm, không xóa/đổi gì trên bảng cũ — nhưng vẫn nên backup trước mọi thay đổi schema).
2. Chạy thử trên 1 bản copy không phải production trước, kiểm tra ứng dụng chạy bình thường.
3. Script tự động chọn dòng `ORCAMENTOCONFIG` đang active, mới tạo gần nhất, để gắn dữ liệu A07/A08 vào đúng kỳ ngân sách — nếu muốn chọn dòng khác, sửa câu lệnh `SELECT` tương ứng trong file (tìm từ khóa `TargetOrcamentoConfigId`) trước khi chạy.
4. Sau khi chạy xong, vào các màn Cấu hình hệ thống để hoàn tất thiết lập riêng của khách hàng (mục 5, phần "Không seed sẵn").

**EN** — File **`Guideline/CONSOLIDATED_client_handoff_2026-07-13.sql`** is a single SQL script, bundled from all 35 original files, reviewed and stripped of anything that only makes sense on the vendor's development machine (e.g. fake test bank accounts, a test year-2099 budget-period row, a test access grant for one specific dev account) — keeping only the real schema and the essential seed data (section 5). Real-tested: run against a clean copy of the original baseline, then the resulting schema was directly diffed against the currently-running database — a 100% match (no missing or extra tables/columns/data types).

How to run it:
1. **Back up your database first** (the script is purely additive — nothing on a pre-existing table is dropped or changed — but always back up before any schema change).
2. Run against a non-production copy first, verify the application still works normally.
3. The script auto-picks the most recently created, currently active `ORCAMENTOCONFIG` row to link the A07/A08 seed data to the right budget period — to pick a different one, edit the corresponding `SELECT` in the file (search for `TargetOrcamentoConfigId`) before running.
4. After it finishes, go through the System Settings screens to complete the client-specific setup (section 5, "Not seeded").

---

## 7. Nguồn / Sources

- 35 file script SQL gốc, theo thứ tự thời gian: `db_migrations/*.sql` (2026-07-11 → 2026-07-13).
- Bản backup gốc dùng để đối chiếu (chụp ngay trước khi bắt đầu): `db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-11_pre_m1.bak`.
- Kết quả so sánh schema được xác minh trực tiếp trên DB (restore bản backup gốc vào DB tạm, so sánh `INFORMATION_SCHEMA.TABLES`/`INFORMATION_SCHEMA.COLUMNS` với DB hiện tại) — không chỉ dựa vào việc đọc lại script. Đã xác minh thêm bằng API thật cho từng luồng chính (Orçamento, Atividade, Classificação Económica, báo cáo CE_INSS_Global, và các màn hình mode cũ liên quan). Xác minh lại 2026-07-13 sau khi có thêm các migration mới, và bằng cách chạy thử script gộp ở mục 6 rồi so sánh schema kết quả với DB hiện tại.

- 35 original SQL scripts, in chronological order: `db_migrations/*.sql` (2026-07-11 → 2026-07-13).
- Baseline backup used for comparison (taken right before work began): `db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-11_pre_m1.bak`.
- The schema comparison was verified directly against the database (baseline restored into a scratch DB, `INFORMATION_SCHEMA.TABLES`/`INFORMATION_SCHEMA.COLUMNS` diffed against the current one) — not just by re-reading the scripts. Further verified via real API calls across the main flows (Orçamento, Atividade, Classificação Económica, the CE_INSS_Global report, and the related old-mode screens). Re-verified 2026-07-13 after more migrations landed, and by running the bundled script from section 6 and diffing the resulting schema against the live database.

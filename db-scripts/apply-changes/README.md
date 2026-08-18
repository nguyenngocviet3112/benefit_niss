# Scripts that change the database

Everything in this folder **writes** to the database. Read the header of each script
before running it, and **back up the database first** — the standard rule for any
change on Staging or Production.

These are not version migrations (those live in `db-migrations/`). They fix or
configure an existing environment, and each one is safe to run more than once: every
step checks the current state first and skips what is already correct.

| Script | What it changes |
|---|---|
| `DB_Health_Check_2026-08-18.sql` | Reports every schema item the current code expects as PRESENT or MISSING, **and creates what is missing**: `PAGAMENTOSEXECUTADOS.bankCode`, `COMPONENTERECEITA_REGISTO.InstitutionId` (+ FK), widens `COMPONENTEDESPESA_REGISTO.descricao` to 150, widens every `ipv6` column below 45. Items belonging to the CE_OSS_Global migration are reported only, not fixed. |
| `Check_Restore_Programa_Roots_2026-08-18.sql` | Read-only **unless** it finds a Programa root coded `04`/`05`/`06` that is inactive and has no `A04`/`A05`/`A06` counterpart — meaning an older copy of the CE_OSS_Global script deactivated live master data. In that case it reactivates that root and everything beneath it. |
| `Enable_Cabimentar_Button_RD03_2026-08-18.sql` | Sets `visualizarDespesaAParaC = 2` on the Despesa component of the **RD03 Cabimento** task, so the **Cabimentar** button appears. Finds the task by name (task ids differ per environment) and stops rather than guessing if zero or several tasks match. Requires a username at the top so the change history records who applied it. The same change can be made through the UI: *Tasks Configure → RD03 task → panel Despesa → "Visualizar Despesas Autorizadas para Cabimentação" → Editável → Gravar*. |

**Configuration does not travel with a code deployment.** Anything here that changes
per-task configuration has to be applied on **every** environment separately —
Dev, Staging and Production.

[VI] Mọi script trong thư mục này đều **ghi** vào database. Đọc phần đầu mỗi script
trước khi chạy và **backup DB trước**. Đây không phải migration theo phiên bản (thứ đó
nằm ở `db-migrations/`), mà là script sửa/cấu hình một môi trường đang chạy; script nào
cũng an toàn khi chạy lại nhiều lần. Lưu ý: cấu hình KHÔNG đi theo code deploy, phải
chạy riêng trên từng môi trường.

# Read-only checks

Scripts in this folder **only read** the database: `SELECT` and `PRINT`, nothing else.
No `INSERT`, `UPDATE`, `DELETE`, `ALTER` or `CREATE`. They are safe to run on any
environment, at any time, as often as needed — including Production.

Use them to find out what state a database is actually in before deciding whether
anything needs to be changed.

| Script | Answers |
|---|---|
| `Diagnose_AD_Saldo_Actividade_2026-08-18.sql` | Why the available budget shown when registering an expense differs from what was expected — which budget line belongs to which Activity, and whether expenses from another budget period are being subtracted. |
| `Diagnose_Cabimentar_Button_2026-08-18.sql` | Why the **Cabimentar** button does not appear on the Cabimento task: prints the per-task Despesa configuration, the state of the expenses in each process, and any expense rows missing Institution / Activity / Functional. |

[VI] Các script trong thư mục này **chỉ đọc** database (`SELECT`/`PRINT`), không ghi
gì cả. An toàn chạy trên mọi môi trường, kể cả Production, chạy bao nhiêu lần cũng được.
Dùng để biết thực trạng trước khi quyết định có cần thay đổi gì không.

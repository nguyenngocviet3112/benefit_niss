-- MỘT LẦN (one-time backfill) — không phải schema migration. Chuyển dữ liệu
-- lịch sử từ MOVIMENTOSBANCARIOS/REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS (bảng
-- cũ, mode cũ) sang BankStatementLine/BankStatementLineGuiaPagamento (bảng
-- mới) — phần cuối của việc hợp nhất Guia Conciliação sang dùng hẳn
-- BankStatementLine (xem 2026-07-13g_bank_statement_line_guia_pagamento.sql
-- và memory bank-statement-line-guia-pagamento-unification).
--
-- Chỉ copy: (1) dòng MOVIMENTOSBANCARIOS đang active, (2) quan hệ khớp Guia
-- Pagamento đang THẬT SỰ active (indActivo = 1, không phải NULL/0 — loại các
-- dòng rác lịch sử không bao giờ hoàn tất, xem memory
-- bank-statement-line-guia-pagamento-unification phần "orphaned rows").
-- Không đụng gì tới bảng cũ (chỉ đọc) — mode cũ vẫn hoạt động bình thường.
--
-- Chạy 1 LẦN DUY NHẤT. Có chống trùng lặp cho phần BankStatementLine (dựa
-- theo cùng quy tắc trùng lặp của tính năng Import Excel — Conta+Data+
-- Credito+Debito+Descricao) nếu lỡ chạy lại, nhưng phần quan hệ Guia Pagamento
-- CHỈ map được cho các dòng vừa insert trong chính lần chạy này — không chạy
-- lại script này sau khi đã chạy thành công.
--
-- Nhớ backup DB trước khi chạy (theo quy ước db_backup/dated_backups).

SET QUOTED_IDENTIFIER ON
GO

-- Bước 1: copy dòng MOVIMENTOSBANCARIOS active chưa có bản sao trong
-- BankStatementLine (so trùng theo Conta+Data+Credito+Debito+Descricao,
-- cùng quy tắc với tính năng Import Excel — script này re-run được, sẽ
-- không tạo trùng lặp nếu chạy lại).
INSERT INTO [dbo].[BankStatementLine]
	([ContaBancariaFk], [DataValor], [Descricao], [Credito], [Debito], [IndActivo], [UtilizadorCriacao], [DataCriacao], [Ipv6])
SELECT src.ContaBancariaFk, src.DataValor, src.Descricao, src.Credito, src.Debito, 1, src.UtilizadorCriacao, src.DataCriacao, src.Ipv6
FROM (
	SELECT m.conta_fk AS ContaBancariaFk, m.dataValor AS DataValor,
	       ISNULL(m.credito, 0) AS Credito, ISNULL(m.debito, 0) AS Debito, m.descricao AS Descricao,
	       ISNULL(m.utilizadorCriacao, 0) AS UtilizadorCriacao, ISNULL(m.dataCriacao, GETDATE()) AS DataCriacao, m.ipv6 AS Ipv6
	FROM [dbo].[MOVIMENTOSBANCARIOS] m
	WHERE m.indActivo = 1 AND m.conta_fk IS NOT NULL
) AS src
WHERE NOT EXISTS (
	SELECT 1 FROM [dbo].[BankStatementLine] bsl
	WHERE bsl.IndActivo = 1
		AND bsl.ContaBancariaFk = src.ContaBancariaFk
		AND bsl.DataValor = src.DataValor
		AND bsl.Credito = src.Credito
		AND bsl.Debito = src.Debito
		AND ISNULL(bsl.Descricao, '') = ISNULL(src.Descricao, '')
);

-- Bước 2: map lại theo cùng bộ field tự nhiên (không dùng OUTPUT — SQL Server
-- không cho OUTPUT tham chiếu cột nguồn của INSERT...SELECT) để tạo quan hệ
-- Guia Pagamento tương ứng cho các dòng vừa/đã copy ở Bước 1.
INSERT INTO [dbo].[BankStatementLineGuiaPagamento]
	([BankStatementLineFk], [GuiaPagamentoFk], [IndActivo], [UtilizadorCriacao], [DataCriacao], [Ipv6])
SELECT bsl.Id, r.guiaPagamento_fk, 1, ISNULL(r.utilizadorCriacao, 0), ISNULL(r.dataCriacao, GETDATE()), r.ipv6
FROM [dbo].[REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS] r
JOIN [dbo].[MOVIMENTOSBANCARIOS] m ON m.id = r.movimentosBancarios_fk
JOIN [dbo].[BankStatementLine] bsl
	ON bsl.IndActivo = 1
	AND bsl.ContaBancariaFk = m.conta_fk
	AND bsl.DataValor = m.dataValor
	AND bsl.Credito = ISNULL(m.credito, 0)
	AND bsl.Debito = ISNULL(m.debito, 0)
	AND ISNULL(bsl.Descricao, '') = ISNULL(m.descricao, '')
WHERE r.indActivo = 1 AND r.guiaPagamento_fk IS NOT NULL
	AND NOT EXISTS (
		SELECT 1 FROM [dbo].[BankStatementLineGuiaPagamento] bslgp
		WHERE bslgp.IndActivo = 1 AND bslgp.BankStatementLineFk = bsl.Id AND bslgp.GuiaPagamentoFk = r.guiaPagamento_fk
	);
GO

-- Adds a bank-account selector to ReceitaPac — user pointed out (2026-07-11)
-- that when Receita is collected via bank, the entry must record WHICH bank
-- account received it, not just a bare "ValorCobradoBanco" amount. Reuses
-- the existing Contabancaria table (also just exposed via the new
-- "Cấu hình hệ thống > Ngân hàng" screen) — nullable since ValorCobradoBanco
-- can be 0 (money entirely via Caixa) and no bank applies then.
--
-- Additive ALTER on a table created THIS SAME engagement (ReceitaPac, added
-- 2026-07-11h) — not a legacy/pre-existing table, so this isn't the kind of
-- "don't modify existing table fields" case the house rule warns about.

ALTER TABLE [dbo].[ReceitaPac] ADD [ContaBancariaFk] [int] NULL
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_ContaBancaria]
	FOREIGN KEY([ContaBancariaFk]) REFERENCES [dbo].[Contabancaria] ([id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_ContaBancaria]
GO

-- Wires the Receita side of the Registo de Lançamentos (sổ nhật ký) — until now
-- only Pagamento (Despesa) auto-generated Débito/Crédito journal entries; Receita
-- had no accounts to book against (see memory lancamentos-conciliacao-link-design,
-- "Receita side still needs new Débito/Crédito columns on ReceitaPac").
--
-- Receita is execution-only (no approval gate — see memory receita-approval-decision),
-- so unlike Pagamento (which has a separate Autorização step to pick the accounts),
-- ReceitaPac picks its own Débito/Crédito accounts directly on the same row it's
-- entered/saved on. Additive ALTER on a table created this engagement — not a
-- legacy/pre-existing table.

ALTER TABLE [dbo].[ReceitaPac] ADD [CodigoContaDebitoFk] [int] NULL
GO
ALTER TABLE [dbo].[ReceitaPac] ADD [CodigoContaCreditoFk] [int] NULL
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_CodigoContaDebito]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_CodigoContaCredito]
GO

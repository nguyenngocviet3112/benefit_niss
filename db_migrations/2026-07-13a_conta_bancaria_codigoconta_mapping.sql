-- Adds a Codigoconta mapping to ContaBancaria (= legacy CONTABANCARIA, same physical
-- table — confirmed via sys.foreign_keys, BankStatementLine.ContaBancariaFk and legacy
-- Movimentosbancarios.conta_fk both point at it). Needed so GuiaConciliacaoDataManager
-- can derive the Débito account DYNAMICALLY from whichever real bank account the matched
-- Movimentosbancarios line actually belongs to, instead of a single fixed config value —
-- confirmed against real 2024 ledger data (SCFSSTL2024_VF.xlsm, sheet "Lançamentos"):
-- every GP collection entry debits the SPECIFIC bank that received the money (1221 BNCTL,
-- 1222 BNU, 1223 Mandiri, 1224 BRI, 1225 ANZ, 1226 BNCTL-OFICIAL, 1211 Banco Central), not
-- one fixed account.
--
-- Also seeds the 7 real banks from that same source (dev DB only had 3 fictional
-- placeholder banks before this) and points each at its correct Codigoconta row.
--
-- Codigoconta data-quality note (investigated, NOT fully fixed here — out of scope for
-- this change): the whole Codigoconta tree has ~106 duplicate full-codes out of 704 rows
-- (2 parallel import batches). For the 7 bank codes used here, one batch (Id 8-14, plus
-- 704 for 1226) matches the source correctly except Id=704's Designacao ("bnf") and Id=12's
-- casing ("MANDIRi") — fixed below since they're directly in scope. The OTHER duplicate
-- batch for these same codes (Id 599-606) is corrupted at every level (parent names wrong
-- too) and is left untouched/unused — nothing references it. The broader ~106-row
-- duplication elsewhere in the tree is unrelated to GP and not addressed here.

UPDATE [dbo].[Codigoconta] SET [Designacao] = 'BNCTL-OFICIAL' WHERE [Id] = 704
GO
UPDATE [dbo].[Codigoconta] SET [Designacao] = 'MANDIRI' WHERE [Id] = 12
GO

ALTER TABLE [dbo].[ContaBancaria] ADD [CodigoContaFk] [int] NULL
GO
ALTER TABLE [dbo].[ContaBancaria] WITH CHECK ADD CONSTRAINT [FK_ContaBancaria_CodigoConta]
	FOREIGN KEY([CodigoContaFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ContaBancaria] CHECK CONSTRAINT [FK_ContaBancaria_CodigoConta]
GO

INSERT INTO [dbo].[ContaBancaria] (Swift, EntidadeBancaria, Descricao, Iban, Numero, CodigoContaFk, UtilizadorCriacao, DataCriacao)
VALUES
	(N'', N'Banco Central', N'', N'', N'', 8, 1, GETDATE()),
	(N'', N'BNCTL', N'', N'', N'', 10, 1, GETDATE()),
	(N'', N'BNU', N'', N'', N'', 11, 1, GETDATE()),
	(N'', N'Mandiri', N'', N'', N'', 12, 1, GETDATE()),
	(N'', N'BRI', N'', N'', N'', 13, 1, GETDATE()),
	(N'', N'ANZ', N'', N'', N'', 14, 1, GETDATE()),
	(N'', N'BNCTL-OFICIAL', N'', N'', N'', 704, 1, GETDATE())
GO

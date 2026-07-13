-- GuiaPagamentoContaConfig.CodigoContaDebitoFk is dropped — it was wrong per real 2024
-- ledger data (SCFSSTL2024_VF.xlsm "Lançamentos" sheet): the Débito side of a GP bank
-- collection is always the SPECIFIC bank account that received the money, not one fixed
-- global account. Débito is now resolved dynamically in GuiaConciliacaoDataManager from
-- ContaBancaria.CodigoContaFk (see 2026-07-13a_conta_bancaria_codigoconta_mapping.sql) of
-- whichever bank line was actually matched. Only CodigoContaCreditoFk (the fixed
-- "Guias Emitidas" receivable-clearing account) remains configurable here.
-- Table is 1 day old with no real data in it (only test rows, already cleaned up) —
-- safe to drop the column outright rather than leaving it as unused/nullable clutter.

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaDebito]
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP COLUMN [CodigoContaDebitoFk]
GO

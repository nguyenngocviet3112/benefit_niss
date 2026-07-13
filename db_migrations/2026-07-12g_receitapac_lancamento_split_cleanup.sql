-- One-off dev-data cleanup, not a schema change (no backup required per house rule,
-- which reserves backups for schema migrations).
--
-- ReceitaPacDataManager.Save used to book the FULL ValorCobradoBanco+ValorCobradoCaixa
-- total under OrigemTipo='ReceitaPac' the moment a Receita was saved (self-reported
-- amount, no bank verification). 2026-07-12 this was split per the same pattern as
-- Guia Pagamento (see memory guia-pagamento-lancamento-wiring):
--   - 'ReceitaPacCaixa' — still booked immediately at Save (no bank statement exists
--     for cash collection, nothing to verify against).
--   - 'ReceitaPacBanco' — now only booked when BankStatementLineDataManager.MatchReceita
--     confirms an exact-amount match against a real bank statement line.
-- The old undifferentiated 'ReceitaPac' OrigemTipo will never be written again. Any
-- pre-existing row under it predates the split and was never bank-verified — left
-- active, it would collide (double-book) with a future correct 'ReceitaPacBanco' entry
-- once that Receita's real bank line gets matched. Deactivate it here; it stays in the
-- table (audit trail) but drops out of the ledger and out of GerarSeChuaCo's
-- ExistsForOrigem check for both the old and new OrigemTipo values (different Id, no
-- collision risk).
--
-- Dev DB only had 1 such row (ReceitaPac Id=3, Numero 3/2026) at the time this ran.

UPDATE [dbo].[Lancamento]
SET [IndActivo] = 0
WHERE [OrigemTipo] = 'ReceitaPac' AND [IndActivo] = 1
GO

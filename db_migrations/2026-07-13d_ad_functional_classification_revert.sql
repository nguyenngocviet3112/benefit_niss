-- Reverts ExpenditureAuthorization.FunctionalClassificationFk added in
-- 2026-07-13c — discovered mid-implementation that OrcamentoLinha (rúbrica)
-- already has a fully backend-wired (Create/Save/MapEntity/Include) but
-- frontend-unexposed FunctionalClassificationFk column, matching exactly how
-- EconomicClassificationFk already works: picked ONCE at rúbrica level, then
-- AD/Cabimento/Compromisso only ever DISPLAY it read-only via the rúbrica
-- chain, never re-pick it. Adding a separate pick-again field on AD would
-- have let the 2 diverge and contradicted the existing Económica precedent.
-- User confirmed (2026-07-13): attach at rúbrica, AD reads through it.
-- No real data was ever written to this column (build-verified only, no
-- AD created via UI with it set) — safe straight drop.

ALTER TABLE [dbo].[ExpenditureAuthorization] DROP CONSTRAINT [FK_ExpenditureAuthorization_FunctionalClassification]
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] DROP COLUMN [FunctionalClassificationFk]
GO

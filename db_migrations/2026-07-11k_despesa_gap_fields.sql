-- Field-completeness gap fix, found via direct cross-check against the real
-- customer paper form supported_documents/Customer provide data/
-- FormulariosDESPESA_emBranco.xlsx (sheets AD/DIC/Compromisso) — flagged by
-- a parallel session's audit (2026-07-11), re-verified directly against the
-- workbook cells before writing this migration (Excel-priority principle).
-- All additive nullable columns — no workflow/state-machine changes.

-- AD sheet, item 1 ("Tipo despesa": Despesa única / Conjunto de despesas)
-- and item 5 ("Solicita-se igualmente autorização para iniciar abertura de
-- procedimento de aprovisionamento": Sim/Não).
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [TipoDespesa] [varchar](20) NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [SolicitaAberturaAprovisionamento] [bit] NULL
GO

-- DIC sheet, item 2 ("Processo aprovisionamento prévio?": Sim/Não) — a
-- DIFFERENT question from AD's item 5 above (this one asks whether a prior
-- procurement process already exists, at the Cabimento stage).
ALTER TABLE [dbo].[Cabimento] ADD [ProcessoAprovisionamentoPrevio] [bit] NULL
GO

-- Compromisso sheet, item 2 ("Compromisso assumido com": Contrato / Listas
-- Beneficiários / Obrigação).
ALTER TABLE [dbo].[CompromissoDespesa] ADD [AssumidoCom] [varchar](30) NULL
GO

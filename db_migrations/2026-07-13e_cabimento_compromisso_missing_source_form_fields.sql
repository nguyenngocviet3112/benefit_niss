-- Same pattern as 2026-07-13c (AD), applied to Cabimento (DIC) and CompromissoDespesa
-- after diffing against FormulariosDESPESA_emBranco.xlsx sheets "DIC" and "Compromisso".
-- User confirmed 2026-07-13.
--
-- Cabimento gaps: Proposta / Fundamentação Legal (source form's "A preencher pelo
-- Departamento Técnico" section) had no home; ApproveComment for the single
-- "Diretor Departamento Financeiro — Validação" sign-off was discarded (Comment
-- already accepted by ApproveCabimentoRequest but only ever stored on the reject path).
-- No FunctionalClassificationFk column needed — displayed read-through via the existing
-- Cabimento -> ExpenditureAuthorization -> OrcamentoLinha chain, same as AD.
--
-- CompromissoDespesa gaps: no missing narrative fields (source form has none), but
-- ReviewComment ("Diretor Departamento Financeiro — Validação") and ApproveComment
-- ("DE — Despacho") were both discarded the same way.

ALTER TABLE [dbo].[Cabimento] ADD [Proposta] [nvarchar](max) NULL
GO
ALTER TABLE [dbo].[Cabimento] ADD [FundamentacaoLegal] [nvarchar](max) NULL
GO
ALTER TABLE [dbo].[Cabimento] ADD [ApproveComment] [nvarchar](1000) NULL
GO

ALTER TABLE [dbo].[CompromissoDespesa] ADD [ReviewComment] [nvarchar](1000) NULL
GO
ALTER TABLE [dbo].[CompromissoDespesa] ADD [ApproveComment] [nvarchar](1000) NULL
GO

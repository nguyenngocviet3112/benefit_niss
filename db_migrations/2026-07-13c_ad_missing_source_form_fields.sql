-- Closes a field gap found via CLAUDE.md §2 diff against the client's original AD form
-- (supported_documents/Customer provide data/FormulariosDESPESA_emBranco.xlsx, sheet "AD"):
-- the AD entry screen was missing 4 fields present in the source form's structure, plus
-- had no way to capture a positive review/approve opinion (only rejection comments were
-- stored — ReviewExpenditureAuthorizationRequest.Comment / ApproveExpenditureAuthorization
-- Request.Comment already existed in the API contract but were silently discarded on the
-- Approve=true path).
--
-- Source form sections covered by the 4 new columns:
--   "Proposta:"                     -> Proposta
--   "Fundamentação Legal:"          -> FundamentacaoLegal
--   "Objetivo da despesa:"          -> ObjetivoDespesa
--   "4.b) Classificação Funcional"  -> FunctionalClassificationFk (master data table
--                                      already existed with real data, just never linked
--                                      from AD)
-- Plus "Pareceres" (2 opinion boxes, filled on a positive Review/Approve, distinct from
-- LastRejectComment which only covers the negative path):
--   ReviewComment, ApproveComment

ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [Proposta] [nvarchar](max) NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [FundamentacaoLegal] [nvarchar](max) NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [ObjetivoDespesa] [nvarchar](max) NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [FunctionalClassificationFk] [int] NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [ReviewComment] [nvarchar](1000) NULL
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] ADD [ApproveComment] [nvarchar](1000) NULL
GO

ALTER TABLE [dbo].[ExpenditureAuthorization] WITH CHECK ADD CONSTRAINT [FK_ExpenditureAuthorization_FunctionalClassification]
	FOREIGN KEY([FunctionalClassificationFk]) REFERENCES [dbo].[FunctionalClassification] ([Id])
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] CHECK CONSTRAINT [FK_ExpenditureAuthorization_FunctionalClassification]
GO

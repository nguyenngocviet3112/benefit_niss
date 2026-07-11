-- OSS perimeter flag + Programa A07/A08, per OSS_Global_2026_FINAL_livro.xlsx
-- (see memory: oss-global-2026-final-livro-findings). A07 "Administração do
-- FRSS" is inside the OSS budget perimeter (same header as A04/A05/A06 in
-- the source workbook); A08 "Regime Contributivo de Capitalização" is
-- explicitly marked "NÃO INTEGRA O PERÍMETRO DO OSS" in the source and must
-- stay OUT of the CE_OSS_Global total while still existing fully in the
-- system (own master-data row, own future report).
-- Run against a DEV/local copy first. Back up with a dated label before
-- running against any shared environment.

ALTER TABLE [dbo].[ProgramActivity] ADD [IsOssPerimeter] [bit] NOT NULL CONSTRAINT [DF_ProgramActivity_IsOssPerimeter] DEFAULT (1)
GO

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A07', 'Administração do FRSS', 1, NULL, 1, 1, 1, 1, GETDATE()),
	('A08', 'Regime Contributivo de Capitalização', 1, NULL, 1, 1, 0, 1, GETDATE())
GO

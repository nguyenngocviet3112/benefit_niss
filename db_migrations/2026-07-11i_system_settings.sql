-- System Settings (Cấu hình hệ thống) — two independent, unrelated pieces:
--   1) LanguageConfig — admin on/off switch for which UI languages are
--      offered, replacing the hardcoded translate.addLangs(['EN','PT','TET','VI'])
--      in AppComponent. Seeded with all 4 active so switching to config-driven
--      selection does not change current behaviour on its own.
--   2) ORCAMENTOCONFIG.ano / .tipo — the table already exists (1 row,
--      Id=1, 2026-01-01 -> 2026-12-31, created manually) but has no Ano
--      column and no way to tell Principal vs Suplementar apart. This adds
--      both; Suplementar (mid-year budget adjustment) creation flow is out
--      of scope this round — only Tipo='PRINCIPAL' rows are created via the
--      new admin screen for now.
-- Back up with a dated label before running against any shared environment
-- (see db_backup/dated_backups/ for the naming convention already used).

-- ============================================================
-- LanguageConfig
-- ============================================================
CREATE TABLE [dbo].[LanguageConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](10) NOT NULL,
	[Nome] [nvarchar](100) NOT NULL,
	[Ordem] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_LanguageConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE UNIQUE INDEX [UX_LanguageConfig_Codigo] ON [dbo].[LanguageConfig] ([Codigo])
GO

-- Seed = exact current hardcoded set or (src/assets/i18n/{EN,PT,TET,VI}.json already exist for
-- all 4) with all 4 marked active, so this migration alone changes nothing
-- user-visible until an admin actually toggles one off.
-- N'' prefix is required on every non-ASCII literal (Português/Tiếng Việt) —
-- without it SQL Server implicitly converts through the server's default
-- codepage before widening to nvarchar, silently mangling the diacritics.
INSERT INTO [dbo].[LanguageConfig] ([Codigo], [Nome], [Ordem], [IndActivo], [UtilizadorCriacao], [DataCriacao])
VALUES
	('EN', N'English', 1, 1, 1, GETDATE()),
	('PT', N'Português', 2, 1, 1, GETDATE()),
	('TET', N'Tetun', 3, 1, 1, GETDATE()),
	('VI', N'Tiếng Việt', 4, 1, 1, GETDATE())
GO

-- ============================================================
-- ORCAMENTOCONFIG.ano / .tipo
-- Column naming follows this table's existing lowercase-first convention
-- (id, dataInicio, indActivo, ...) — see modelBuilder.Entity<Orcamentoconfig>
-- in TimorINSSModuloContribuicoesContext.cs.
-- ============================================================
ALTER TABLE [dbo].[ORCAMENTOCONFIG] ADD [ano] [int] NULL
GO
ALTER TABLE [dbo].[ORCAMENTOCONFIG] ADD [tipo] [varchar](20) NULL
GO

UPDATE [dbo].[ORCAMENTOCONFIG]
SET [ano] = YEAR([dataInicio]),
    [tipo] = 'PRINCIPAL'
WHERE [ano] IS NULL
GO

ALTER TABLE [dbo].[ORCAMENTOCONFIG] ALTER COLUMN [ano] [int] NOT NULL
GO
ALTER TABLE [dbo].[ORCAMENTOCONFIG] ALTER COLUMN [tipo] [varchar](20) NOT NULL
GO

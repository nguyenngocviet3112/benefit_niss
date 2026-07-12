-- BudgetPeriod — dedicated "Kỳ ngân sách" table for the new mode, replacing the
-- reuse of the pre-existing [Orcamentoconfig] (which old-mode still uses as its
-- own year-anchor for [CODIGOCONTA]/[CENTROCUSTO]/[RELTIPODECONTAORCAMENTOCONFIG]/
-- [COMPONENTEORCAMENTO_REGISTO] — see db_migrations/2026-07-11i_system_settings.sql,
-- which added Ano/Tipo directly onto [Orcamentoconfig] and is now reverted below).
--
-- Decision (2026-07-12, user-requested): new-mode tables should never touch a
-- pre-existing table's schema when a dedicated satellite table is just as easy —
-- this build is pre-handoff (nothing delivered to the client yet), so there is no
-- production data-migration risk in making this switch now rather than carrying
-- the coupling forward. [Orcamentoconfig] returns to its exact pre-engagement
-- shape; [ProgramActivity]/[EconomicClassification]/[OrcamentoBatch]/
-- [OrcamentoSuplementar] now anchor to [BudgetPeriod] instead.
--
-- Wire-format field name intentionally UNCHANGED: the ~30 consumer files (request/
-- response DTOs, Angular services/components across Orçamento, AD/Cabimento,
-- Receita, Saldos de Abertura, Plano de Contas, master-data import, etc.) keep
-- passing a field literally named "orcamentoConfigFk" that now means "id of a
-- BudgetPeriod row" — only the DB column/C# model property actually being
-- migrated (on the 4 owning tables) is renamed to BudgetPeriodFk. Renaming the
-- wire format too would touch ~30 files with zero functional benefit.
--
-- Run against a DEV/local copy first. Back up with a dated label before running
-- against any shared environment.

CREATE TABLE [dbo].[BudgetPeriod](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ano] [int] NOT NULL,
	[Tipo] [varchar](20) NOT NULL,
	[DataInicio] [datetime] NOT NULL,
	[DataFim] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_BudgetPeriod] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Seed rows matching the 2 existing [Orcamentoconfig] rows (same Id values, so
-- every existing ProgramActivity/EconomicClassification/OrcamentoBatch/
-- OrcamentoSuplementar row's *ConfigFk=1|2 keeps resolving correctly once the FK
-- columns below are repointed) — this is a straight IDENTITY_INSERT copy, not a
-- reinterpretation of the data.
SET IDENTITY_INSERT [dbo].[BudgetPeriod] ON
INSERT INTO [dbo].[BudgetPeriod] ([Id], [Ano], [Tipo], [DataInicio], [DataFim], [IndActivo], [UtilizadorCriacao], [DataCriacao])
VALUES
	(1, 2026, 'PRINCIPAL', '2026-01-01', '2026-12-31', 1, 1, GETDATE()),
	(2, 2099, 'PRINCIPAL', '2099-01-01', '2099-12-31', 1, 1007, GETDATE())
SET IDENTITY_INSERT [dbo].[BudgetPeriod] OFF
GO

-- Keep the IDENTITY seed ahead of any future insert (SET IDENTITY_INSERT ON does
-- not itself advance the seed for the next auto-generated Id).
DBCC CHECKIDENT ('[dbo].[BudgetPeriod]', RESEED, 2)
GO

-- Repoint ProgramActivity -----------------------------------------------------
ALTER TABLE [dbo].[ProgramActivity] DROP CONSTRAINT [FK_ProgramActivity_OrcamentoConfig]
GO
EXEC sp_rename 'dbo.ProgramActivity.OrcamentoConfigFk', 'BudgetPeriodFk', 'COLUMN'
GO
ALTER TABLE [dbo].[ProgramActivity] WITH CHECK ADD CONSTRAINT [FK_ProgramActivity_BudgetPeriod]
	FOREIGN KEY([BudgetPeriodFk]) REFERENCES [dbo].[BudgetPeriod] ([Id])
GO

-- Repoint EconomicClassification -----------------------------------------------
ALTER TABLE [dbo].[EconomicClassification] DROP CONSTRAINT [FK_EconomicClassification_OrcamentoConfig]
GO
EXEC sp_rename 'dbo.EconomicClassification.OrcamentoConfigFk', 'BudgetPeriodFk', 'COLUMN'
GO
ALTER TABLE [dbo].[EconomicClassification] WITH CHECK ADD CONSTRAINT [FK_EconomicClassification_BudgetPeriod]
	FOREIGN KEY([BudgetPeriodFk]) REFERENCES [dbo].[BudgetPeriod] ([Id])
GO

-- Repoint OrcamentoBatch --------------------------------------------------------
ALTER TABLE [dbo].[OrcamentoBatch] DROP CONSTRAINT [FK_OrcamentoBatch_OrcamentoConfig]
GO
EXEC sp_rename 'dbo.OrcamentoBatch.OrcamentoConfigFk', 'BudgetPeriodFk', 'COLUMN'
GO
ALTER TABLE [dbo].[OrcamentoBatch] WITH CHECK ADD CONSTRAINT [FK_OrcamentoBatch_BudgetPeriod]
	FOREIGN KEY([BudgetPeriodFk]) REFERENCES [dbo].[BudgetPeriod] ([Id])
GO

-- Repoint OrcamentoSuplementar ---------------------------------------------------
ALTER TABLE [dbo].[OrcamentoSuplementar] DROP CONSTRAINT [FK_OrcamentoSuplementar_OrcamentoConfig]
GO
EXEC sp_rename 'dbo.OrcamentoSuplementar.OrcamentoConfigFk', 'BudgetPeriodFk', 'COLUMN'
GO
ALTER TABLE [dbo].[OrcamentoSuplementar] WITH CHECK ADD CONSTRAINT [FK_OrcamentoSuplementar_BudgetPeriod]
	FOREIGN KEY([BudgetPeriodFk]) REFERENCES [dbo].[BudgetPeriod] ([Id])
GO

-- Revert [Orcamentoconfig] back to its exact pre-engagement shape ---------------
-- (added by 2026-07-11i_system_settings.sql, no longer needed — see
-- orcamentoconfig-notnull-column-regression memory for the old-mode regression
-- this caused and its now-moot fix in CamposEditaveisDataManager, also reverted).
ALTER TABLE [dbo].[ORCAMENTOCONFIG] DROP COLUMN [ano]
GO
ALTER TABLE [dbo].[ORCAMENTOCONFIG] DROP COLUMN [tipo]
GO

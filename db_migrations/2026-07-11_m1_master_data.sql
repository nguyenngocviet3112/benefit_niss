-- M1 Master Data — additive tables for the new Contabilidade mode.
-- Naming: new tables use English names + PascalCase columns (no explicit
-- Fluent config in EF, same convention already used for [Institution]).
-- Run against a DEV/local copy first. Back up with a dated label before
-- running against any shared environment.

-- ============================================================
-- ProgramActivity — Programa -> Subprograma -> Atividade tree
-- Versioned per year via OrcamentoConfigFk (same pattern as Codigoconta).
-- ============================================================
CREATE TABLE [dbo].[ProgramActivity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](20) NOT NULL,
	[Designacao] [varchar](200) NOT NULL,
	[Nivel] [int] NOT NULL,
	[ParentFk] [int] NULL,
	[OrcamentoConfigFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ProgramActivity] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProgramActivity] WITH CHECK ADD CONSTRAINT [FK_ProgramActivity_Parent]
	FOREIGN KEY([ParentFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[ProgramActivity] CHECK CONSTRAINT [FK_ProgramActivity_Parent]
GO

ALTER TABLE [dbo].[ProgramActivity] WITH CHECK ADD CONSTRAINT [FK_ProgramActivity_OrcamentoConfig]
	FOREIGN KEY([OrcamentoConfigFk]) REFERENCES [dbo].[ORCAMENTOCONFIG] ([id])
GO
ALTER TABLE [dbo].[ProgramActivity] CHECK CONSTRAINT [FK_ProgramActivity_OrcamentoConfig]
GO

CREATE UNIQUE INDEX [UX_ProgramActivity_Codigo_Ano] ON [dbo].[ProgramActivity] ([Codigo], [OrcamentoConfigFk])
GO

-- ============================================================
-- FunctionalClassification — COFOG-style, 2 levels, evergreen
-- (no OrcamentoConfigFk — stable across years, per spec).
-- ============================================================
CREATE TABLE [dbo].[FunctionalClassification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](20) NOT NULL,
	[Designacao] [varchar](200) NOT NULL,
	[ParentFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_FunctionalClassification] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FunctionalClassification] WITH CHECK ADD CONSTRAINT [FK_FunctionalClassification_Parent]
	FOREIGN KEY([ParentFk]) REFERENCES [dbo].[FunctionalClassification] ([Id])
GO
ALTER TABLE [dbo].[FunctionalClassification] CHECK CONSTRAINT [FK_FunctionalClassification_Parent]
GO

CREATE UNIQUE INDEX [UX_FunctionalClassification_Codigo] ON [dbo].[FunctionalClassification] ([Codigo])
GO

-- ============================================================
-- Institution — Organization dimension (INSS/FRSS).
-- VERIFIED 2026-07-11 against the live dev DB: table [dbo].[INSTITUTION]
-- ALREADY EXISTS (created 2025-12-14, presumably by the colleague who wrote
-- the C# stack) with lowercase-style columns (id, nome, indActivo,
-- utilizadorCriacao, dataCriacao, utilizadorAlteracao, dataAlteracao, ipv6)
-- — NOT the PascalCase this script originally assumed. It already contains
-- the exact 2 seed rows this script would have added: Id=1 'INSS', Id=2
-- 'FRSS'. Works fine with the existing PascalCase-referencing EF Core code
-- because the server collation is SQL_Latin1_General_CP1_CI_AS (case-
-- insensitive), so [Id]/[Nome] in generated SQL still match [id]/[nome].
-- NOTHING TO DO HERE — table + seed already present. Kept this note instead
-- of the original CREATE TABLE/INSERT so this script stays idempotent if
-- re-run against a DB that already has it (which is every environment so far).
-- ============================================================

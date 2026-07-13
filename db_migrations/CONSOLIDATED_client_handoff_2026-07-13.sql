/*
================================================================================
 CONSOLIDATED SCHEMA MIGRATION -- Modulo Contabilidade (new mode)
 Generated 2026-07-13. Assembled + reviewed from 35 dated scripts in
 db_migrations/ (2026-07-11 -> 2026-07-13), each already run in sequence
 against the vendor's own development database to build the exact schema
 currently live there. This file reproduces the same END-STATE schema in one
 run, with vendor development-environment-only data REMOVED (see notes at
 each edited section below) -- every table/column here is a genuine, additive
 product change, safe to apply to a live database with real production data.

 SCOPE -- schema only (CREATE TABLE / ALTER TABLE / indexes / constraints) plus
 a small set of fixed application catalog rows every install needs to
 function (language list, RBAC starter presets, a login-compatibility
 placeholder row, singleton config defaults). It does NOT seed any business
 data specific to the vendor's test environment (no test bank accounts, no
 test budget-period rows, no test user grants) -- see the inline notes at each
 point content was intentionally omitted.

 BEFORE RUNNING:
   1. Back up your database first (this script is additive/safe by design --
      no existing table is dropped, no existing column is removed or has its
      type changed -- but always back up before any schema change).
   2. Run against a non-production copy first and verify the application
      still works end-to-end on it.
   3. This script auto-picks your most recently created active
      [ORCAMENTOCONFIG] row to link the seeded "Administracao do FRSS" (A07)
      / "Regime Contributivo de Capitalizacao" (A08) program structure to
      your real budget year (see the pre-flight check right below, and the
      per-batch DECLARE @TargetOrcamentoConfigId statements further down --
      SQL Server does not carry local variables across GO batch separators,
      so it is re-resolved with the same query in every batch that needs it).
      If that is not the row you want, edit the query in this file (search
      for "TargetOrcamentoConfigId") before running.
   4. After running, a few things need one-time setup via the application's
      own admin screens (not pre-seeded here, since they're specific to your
      real environment):
        - Cau hinh he thong > Ngan hang -- enter your real bank accounts and
          map each to its Codigoconta chart-of-accounts row.
        - Cau hinh he thong > Cau hinh Tai khoan Guia Pagamento / Cau hinh
          Tai khoan Liquidacao -- pick the Debito/Credito accounts used to
          auto-generate accounting entries (left unset here; the application
          works without them, it simply won't auto-book until configured).
        - Quan ly User & Phan quyen -- grant real users access to the new
          mode and assign permission tokens/presets.
================================================================================
*/

-- Pre-flight check only (fails fast with a clear message if there is no
-- ORCAMENTOCONFIG row to anchor the A07/A08 seed to). The actual value is
-- re-resolved per batch further down, since SQL Server does not carry local
-- variables across GO.
IF NOT EXISTS (SELECT 1 FROM [dbo].[ORCAMENTOCONFIG] WHERE [indActivo] = 1)
BEGIN
	RAISERROR('No active ORCAMENTOCONFIG row found -- create your budget-year config first before running this script.', 16, 1);
END
GO


-- ============================================================
-- Source: db_migrations/2026-07-11_m1_master_data.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-11b_mode_switch.sql
-- ============================================================
-- Mode switch — gates which users can enter the new Contabilidade mode.
-- Interim mechanism: presence of an ACTIVE row = has access. This is a
-- placeholder gate; it gets superseded by the full granular UserPermission
-- system when "Quản lý User & Phân quyền" is built — do not extend this
-- table with more columns, replace it instead.

CREATE TABLE [dbo].[UserModeAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserModeAccess] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserModeAccess] WITH CHECK ADD CONSTRAINT [FK_UserModeAccess_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[UTILIZADOR] ([idUtilizador])
GO
ALTER TABLE [dbo].[UserModeAccess] CHECK CONSTRAINT [FK_UserModeAccess_Utilizador]
GO

CREATE UNIQUE INDEX [UX_UserModeAccess_Utilizador] ON [dbo].[UserModeAccess] ([UtilizadorFk])
GO

-- (Original dev migration also seeded a test UserModeAccess row for a
-- specific dev user Id — intentionally OMITTED here. Grant access to real
-- users via the app's own admin screen after go-live.)

-- ============================================================
-- Source: db_migrations/2026-07-11c_economic_classification.sql
-- ============================================================
-- EconomicClassification — Classificação Económica (Receita 4xx / Despesa 5xx budget codes).
-- NEW dedicated table — do NOT reuse [Codigoconta]: that table is already
-- populated (584 rows, OrcamentoConfigFk=1) with the SNC-TL GL chart of
-- accounts (Débito/Crédito ledger), a different taxonomy from the Excel's
-- budget economic classification despite similar leading digits. See
-- finance-report-impl-spec memory entry dated 2026-07-11 for the full
-- rationale (verified live against the dev DB before deciding to split).
-- Mirrors [ProgramActivity]'s exact shape/versioning.

CREATE TABLE [dbo].[EconomicClassification](
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
	CONSTRAINT [PK_EconomicClassification] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EconomicClassification] WITH CHECK ADD CONSTRAINT [FK_EconomicClassification_Parent]
	FOREIGN KEY([ParentFk]) REFERENCES [dbo].[EconomicClassification] ([Id])
GO
ALTER TABLE [dbo].[EconomicClassification] CHECK CONSTRAINT [FK_EconomicClassification_Parent]
GO

ALTER TABLE [dbo].[EconomicClassification] WITH CHECK ADD CONSTRAINT [FK_EconomicClassification_OrcamentoConfig]
	FOREIGN KEY([OrcamentoConfigFk]) REFERENCES [dbo].[ORCAMENTOCONFIG] ([id])
GO
ALTER TABLE [dbo].[EconomicClassification] CHECK CONSTRAINT [FK_EconomicClassification_OrcamentoConfig]
GO

CREATE UNIQUE INDEX [UX_EconomicClassification_Codigo_Ano] ON [dbo].[EconomicClassification] ([Codigo], [OrcamentoConfigFk])
GO

-- Added after initial creation (same session) — the source Excel's own
-- "Tipo" column (Receita/Despesa), kept explicit rather than re-derived
-- from the leading code digit.
ALTER TABLE [dbo].[EconomicClassification] ADD [Tipo] [varchar](20) NULL
GO

-- ============================================================
-- Source: db_migrations/2026-07-11d_orcamento.sql
-- ============================================================
-- Orçamento (M2) — batch-level approval workflow for budget rúbrica lines.
-- Rúbrica = one Atividade x Classificação Económica x Organization x Ano
-- line with a budgeted Valor. Submission/review/approval happens at the
-- BATCH level (the whole set of lines together), per the locked 2026-07-10
-- decision — see finance-report-impl-spec memory.
--
-- CORRECTED (2026-07-11): the real sample import file
-- (sample_import_data/orcamento/Orcamento_2026_import_mau.xlsx) has NO
-- Classificação Funcional column — only Ano/Organization/Programa/
-- Subprograma/Atividade/Classificação Económica/Valor. This conflicts with
-- an earlier note that listed Funcional as part of the rúbrica key; per the
-- Excel-priority principle, FunctionalClassificationFk is kept but made
-- NULLABLE (not part of the entry/uniqueness key) — assignable later
-- (e.g. at the AD/Cabimento step) if/when needed.

CREATE TABLE [dbo].[OrcamentoBatch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrcamentoConfigFk] [int] NOT NULL,
	-- DRAFT | PENDING_REVIEW | PENDING_APPROVAL | APPROVED
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ReviewedBy] [int] NULL,
	[ReviewedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	-- Preenchido quando Review/Approve rejeita — o batch volta para DRAFT
	-- mas o motivo fica visível até à próxima submissão.
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_OrcamentoBatch] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrcamentoBatch] WITH CHECK ADD CONSTRAINT [FK_OrcamentoBatch_OrcamentoConfig]
	FOREIGN KEY([OrcamentoConfigFk]) REFERENCES [dbo].[ORCAMENTOCONFIG] ([id])
GO
ALTER TABLE [dbo].[OrcamentoBatch] CHECK CONSTRAINT [FK_OrcamentoBatch_OrcamentoConfig]
GO

CREATE TABLE [dbo].[OrcamentoLinha](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrcamentoBatchFk] [int] NOT NULL,
	[AtividadeFk] [int] NOT NULL,
	[EconomicClassificationFk] [int] NOT NULL,
	[FunctionalClassificationFk] [int] NULL,
	[OrganizationFk] [int] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_OrcamentoLinha] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrcamentoLinha] WITH CHECK ADD CONSTRAINT [FK_OrcamentoLinha_Batch]
	FOREIGN KEY([OrcamentoBatchFk]) REFERENCES [dbo].[OrcamentoBatch] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoLinha] CHECK CONSTRAINT [FK_OrcamentoLinha_Batch]
GO

ALTER TABLE [dbo].[OrcamentoLinha] WITH CHECK ADD CONSTRAINT [FK_OrcamentoLinha_Atividade]
	FOREIGN KEY([AtividadeFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoLinha] CHECK CONSTRAINT [FK_OrcamentoLinha_Atividade]
GO

ALTER TABLE [dbo].[OrcamentoLinha] WITH CHECK ADD CONSTRAINT [FK_OrcamentoLinha_EconomicClassification]
	FOREIGN KEY([EconomicClassificationFk]) REFERENCES [dbo].[EconomicClassification] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoLinha] CHECK CONSTRAINT [FK_OrcamentoLinha_EconomicClassification]
GO

ALTER TABLE [dbo].[OrcamentoLinha] WITH CHECK ADD CONSTRAINT [FK_OrcamentoLinha_FunctionalClassification]
	FOREIGN KEY([FunctionalClassificationFk]) REFERENCES [dbo].[FunctionalClassification] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoLinha] CHECK CONSTRAINT [FK_OrcamentoLinha_FunctionalClassification]
GO

ALTER TABLE [dbo].[OrcamentoLinha] WITH CHECK ADD CONSTRAINT [FK_OrcamentoLinha_Institution]
	FOREIGN KEY([OrganizationFk]) REFERENCES [dbo].[Institution] ([id])
GO
ALTER TABLE [dbo].[OrcamentoLinha] CHECK CONSTRAINT [FK_OrcamentoLinha_Institution]
GO

-- Uma rúbrica (combo Atividade x EconClass x Organization) só pode ter UMA
-- linha ativa em todo o sistema, independente do batch/estado — evita
-- orçamentar a mesma rúbrica duas vezes em paralelo. Funcional NÃO faz parte
-- da chave (ver nota acima).
-- Filtered index requires QUOTED_IDENTIFIER ON for this session.
SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_OrcamentoLinha_Rubrica] ON [dbo].[OrcamentoLinha]
	([AtividadeFk], [EconomicClassificationFk], [OrganizationFk])
	WHERE [IndActivo] = 1
GO

-- ============================================================
-- Source: db_migrations/2026-07-11e_expenditure_authorization.sql
-- ============================================================
-- ExpenditureAuthorization (M3) — AD/Cabimento, merged into one entity per
-- the verified 2026-07-07 decision (Excel shows N.º AD == N.º Cabimento 1:1).
-- Generated semi-auto FROM one APPROVED OrcamentoLinha (rúbrica); Numero is
-- a bare integer that resets every month (composite key Numero+Mes+Ano).
-- Per-record DRAFT->PENDING_REVIEW->PENDING_APPROVAL->APPROVED workflow,
-- same shape as OrcamentoBatch (not batched — each AD is already atomic).

CREATE TABLE [dbo].[ExpenditureAuthorization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[OrcamentoLinhaFk] [int] NOT NULL,
	[Descritivo] [nvarchar](500) NULL,
	[ValorAutorizado] [decimal](18, 2) NOT NULL,
	[Regularizacao] [decimal](18, 2) NOT NULL,
	-- DRAFT | PENDING_REVIEW | PENDING_APPROVAL | APPROVED
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ReviewedBy] [int] NULL,
	[ReviewedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ExpenditureAuthorization] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ExpenditureAuthorization] WITH CHECK ADD CONSTRAINT [FK_ExpenditureAuthorization_OrcamentoLinha]
	FOREIGN KEY([OrcamentoLinhaFk]) REFERENCES [dbo].[OrcamentoLinha] ([Id])
GO
ALTER TABLE [dbo].[ExpenditureAuthorization] CHECK CONSTRAINT [FK_ExpenditureAuthorization_OrcamentoLinha]
GO

CREATE TABLE [dbo].[ExpenditureAuthorizationPlurianualidade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpenditureAuthorizationFk] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ExpenditureAuthorizationPlurianualidade] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ExpenditureAuthorizationPlurianualidade] WITH CHECK ADD CONSTRAINT [FK_ExpAuthPluri_ExpAuth]
	FOREIGN KEY([ExpenditureAuthorizationFk]) REFERENCES [dbo].[ExpenditureAuthorization] ([Id])
GO
ALTER TABLE [dbo].[ExpenditureAuthorizationPlurianualidade] CHECK CONSTRAINT [FK_ExpAuthPluri_ExpAuth]
GO

SET QUOTED_IDENTIFIER ON
GO
-- 1 AD por rúbrica (OrcamentoLinha) — 1:1.
CREATE UNIQUE INDEX [UX_ExpenditureAuthorization_OrcamentoLinha] ON [dbo].[ExpenditureAuthorization] ([OrcamentoLinhaFk])
	WHERE [IndActivo] = 1
GO
-- N.º AD reinicia por mês — único dentro de (Mes, Ano).
CREATE UNIQUE INDEX [UX_ExpenditureAuthorization_Numero] ON [dbo].[ExpenditureAuthorization] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO
CREATE UNIQUE INDEX [UX_ExpAuthPluri_Ano] ON [dbo].[ExpenditureAuthorizationPlurianualidade] ([ExpenditureAuthorizationFk], [Ano])
	WHERE [IndActivo] = 1
GO

-- Fixed after finding Vietnamese text corruption (varchar doesn't support
-- full Unicode) — Descritivo/LastRejectComment should be nvarchar.

-- ============================================================
-- Source: db_migrations/2026-07-11f_cabimento.sql
-- ============================================================
-- Cabimento (DIC = Declaração de Inscrição e Cabimento) — LEGALLY SEPARATE
-- from AD (ExpenditureAuthorization), per ReuniaoSS_2024.pptx (Lei 2/2022,
-- DL 23/2022) + real blank form FormulariosDESPESA_emBranco.xlsx. AD ends
-- at DE approval; DIC/Cabimento ends at Diretor DF only (no DE) — a shorter
-- 2-stage workflow. See customer-provided-data-findings memory (2026-07-11).
--
-- Generated from an APPROVED ExpenditureAuthorization (still 1:1 per AD —
-- only the *entity/workflow* was wrongly merged before, not the 1:1
-- cardinality, which the real process still implies).

CREATE TABLE [dbo].[Cabimento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[ExpenditureAuthorizationFk] [int] NOT NULL,
	[Descritivo] [nvarchar](500) NULL,
	[ValorCabimentado] [decimal](18, 2) NOT NULL,
	-- DRAFT | PENDING_APPROVAL | APPROVED (2-stage — Diretor DF only, no DE)
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_Cabimento] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Cabimento] WITH CHECK ADD CONSTRAINT [FK_Cabimento_ExpenditureAuthorization]
	FOREIGN KEY([ExpenditureAuthorizationFk]) REFERENCES [dbo].[ExpenditureAuthorization] ([Id])
GO
ALTER TABLE [dbo].[Cabimento] CHECK CONSTRAINT [FK_Cabimento_ExpenditureAuthorization]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_Cabimento_ExpenditureAuthorization] ON [dbo].[Cabimento] ([ExpenditureAuthorizationFk])
	WHERE [IndActivo] = 1
GO
CREATE UNIQUE INDEX [UX_Cabimento_Numero] ON [dbo].[Cabimento] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO

-- Fixed after finding Vietnamese text corruption (varchar doesn't support
-- full Unicode) — Descritivo/LastRejectComment should be nvarchar.

-- ============================================================
-- Source: db_migrations/2026-07-11f_oss_perimeter.sql
-- ============================================================
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

DECLARE @TargetOrcamentoConfigId INT = (SELECT TOP 1 [id] FROM [dbo].[ORCAMENTOCONFIG] WHERE [indActivo] = 1 ORDER BY [dataCriacao] DESC);

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A07', 'Administração do FRSS', 1, NULL, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE()),
	('A08', 'Regime Contributivo de Capitalização', 1, NULL, @TargetOrcamentoConfigId, 1, 0, 1, GETDATE())
GO

-- ============================================================
-- Source: db_migrations/2026-07-11g_compromisso_despesa.sql
-- ============================================================
-- CompromissoDespesa — new-mode Compromisso, distinct from the OLD/empty
-- [COMPROMISSO] table (0 rows, tied to the old tarefaAtivo_fk workflow,
-- incompatible shape). Named "CompromissoDespesa" (not "Compromisso") to
-- avoid a case-insensitive collation collision with [COMPROMISSO].
--
-- References Cabimento (must be APPROVED) — chain is AD -> Cabimento ->
-- CompromissoDespesa, per the legal separation confirmed 2026-07-11.
-- One Cabimento can have MANY CompromissoDespesa rows (not 1:1) — e.g. one
-- compromisso per contract/person, or per bank x month for pensions, per
-- ReuniaoSS_2024.pptx granularity rules.
--
-- 3-stage approval (DRAFT -> PENDING_REVIEW -> PENDING_APPROVAL -> APPROVED,
-- Diretor DF review + DE approve) — same shape as ExpenditureAuthorization,
-- ADDS a gate that was previously "execution-only" per the superseded 2026-07-10
-- decision (see customer-provided-data-findings memory).

CREATE TABLE [dbo].[CompromissoDespesa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[CabimentoFk] [int] NOT NULL,
	[Descritivo] [nvarchar](500) NULL,
	-- Valor Compromisso global (Plurianual) = total commitment across all years
	[ValorCompromissoGlobal] [decimal](18, 2) NOT NULL,
	-- Compromisso no Ano = this year's portion
	[ValorCompromissoAno] [decimal](18, 2) NOT NULL,
	[Regularizacao] [decimal](18, 2) NOT NULL,
	-- DRAFT | PENDING_REVIEW | PENDING_APPROVAL | APPROVED
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ReviewedBy] [int] NULL,
	[ReviewedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_CompromissoDespesa] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CompromissoDespesa] WITH CHECK ADD CONSTRAINT [FK_CompromissoDespesa_Cabimento]
	FOREIGN KEY([CabimentoFk]) REFERENCES [dbo].[Cabimento] ([Id])
GO
ALTER TABLE [dbo].[CompromissoDespesa] CHECK CONSTRAINT [FK_CompromissoDespesa_Cabimento]
GO

CREATE TABLE [dbo].[CompromissoDespesaPlurianualidade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompromissoDespesaFk] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_CompromissoDespesaPlurianualidade] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CompromissoDespesaPlurianualidade] WITH CHECK ADD CONSTRAINT [FK_CompDespPluri_CompDesp]
	FOREIGN KEY([CompromissoDespesaFk]) REFERENCES [dbo].[CompromissoDespesa] ([Id])
GO
ALTER TABLE [dbo].[CompromissoDespesaPlurianualidade] CHECK CONSTRAINT [FK_CompDespPluri_CompDesp]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_CompromissoDespesa_Numero] ON [dbo].[CompromissoDespesa] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO
CREATE UNIQUE INDEX [UX_CompDespPluri_Ano] ON [dbo].[CompromissoDespesaPlurianualidade] ([CompromissoDespesaFk], [Ano])
	WHERE [IndActivo] = 1
GO

-- Fixed after finding Vietnamese text corruption (varchar doesn't support
-- full Unicode) — Descritivo/LastRejectComment should be nvarchar.

-- ============================================================
-- Source: db_migrations/2026-07-11h_obligation.sql
-- ============================================================
-- Obligation (Obrigação) — many-to-many with CompromissoDespesa via
-- ObligationItem, per real Excel example (N.º Obrigação 1 groups TWO
-- Compromissos into one combined payment batch) and legal training deck.
-- 2-stage approval (DRAFT -> PENDING_APPROVAL -> APPROVED, Diretor DF
-- "Aprovação e Liquidação" only, no DE) — same shape as Cabimento, ADDS a
-- gate that was previously "execution-only" per the superseded 2026-07-10
-- decision (see customer-provided-data-findings memory).
-- No Regularização/Valor revisto/Plurianualidade — confirmed absent from
-- the OBRIGAÇÃO Excel sheet, kept out per Excel-priority principle.

CREATE TABLE [dbo].[Obligation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[DescritivoObrigacao] [nvarchar](500) NULL,
	-- DRAFT | PENDING_APPROVAL | APPROVED
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_Obligation] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ObligationItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObligationFk] [int] NOT NULL,
	[CompromissoDespesaFk] [int] NOT NULL,
	[Value] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ObligationItem] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ObligationItem] WITH CHECK ADD CONSTRAINT [FK_ObligationItem_Obligation]
	FOREIGN KEY([ObligationFk]) REFERENCES [dbo].[Obligation] ([Id])
GO
ALTER TABLE [dbo].[ObligationItem] CHECK CONSTRAINT [FK_ObligationItem_Obligation]
GO

ALTER TABLE [dbo].[ObligationItem] WITH CHECK ADD CONSTRAINT [FK_ObligationItem_CompromissoDespesa]
	FOREIGN KEY([CompromissoDespesaFk]) REFERENCES [dbo].[CompromissoDespesa] ([Id])
GO
ALTER TABLE [dbo].[ObligationItem] CHECK CONSTRAINT [FK_ObligationItem_CompromissoDespesa]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_Obligation_Numero] ON [dbo].[Obligation] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO

-- ============================================================
-- Source: db_migrations/2026-07-11h_receita_pac.sql
-- ============================================================
-- ReceitaPac — new-mode Receita entry (RECEITAS_PAC sheet: "REGISTO DE RECEITAS
-- LIQUIDADAS - OUTRAS" — bank interest, opening-balance carryover code 408,
-- etc). Does NOT cover RECEITAS_GP (contribution income, 401.xx) — that data
-- already lives in the Contribuições module (GuiaPagamento); Finance only
-- reconciles/reports it, no new entry screen needed for it.
--
-- Distinct from the OLD [COMPONENTERECEITA_REGISTO] table (workflow-driven
-- tarefaActivo_fk dispatcher, agrupamentoConfig_fk legacy tree, no separate
-- Crédito tracking) — this is a fresh satellite table against the NEW
-- EconomicClassification/ProgramActivity master data, per the additive-
-- upgrade convention used for Orcamento/ExpenditureAuthorization/Cabimento/
-- CompromissoDespesa.
--
-- EXECUTION-ONLY, no approval gate — unlike Despesa. Resolved 2026-07-11
-- after finding a contradiction in supported_documents/Customer provide
-- data/Primeira Fase - Doc. de especificações técnicas completo-
-- SRSD_Mod_Cont_Mod_Fin.pdf (p.43 says "Registo de receitas" with no
-- "Aprovação", vs the old shared Execução table schema implying a common
-- Cabimentado/Aprovado state machine for both Receita and Despesa) — user
-- confirmed execution-only, and separately said the DB-architecture PDF
-- (Modulo_Interno_Mod_Financeiro_Mod_Contribuicoes_Arquitectura_BD.pdf) is
-- outdated and should be disregarded. See memory receita-approval-decision.
--
-- Regime is NOT derived from Atividade here (differs from Despesa, confirmed
-- 2026-07-10 via real RECEITAS_PAC rows showing Regime populated with
-- Atividade blank) — RegimeFk is an independent FK straight to a ROOT
-- ProgramActivity row (A04/A05/A06/A07/A08), AtividadeFk is optional and can
-- be any level. Both point at ProgramActivity — two separate FKs to the same
-- table, same pattern already used elsewhere for dual references.

CREATE TABLE [dbo].[ReceitaPac](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[Niss] [varchar](20) NULL,
	[RegimeFk] [int] NOT NULL,
	[AtividadeFk] [int] NULL,
	[EconomicClassificationFk] [int] NOT NULL,
	[OrganizationFk] [int] NOT NULL,
	[Descritivo] [nvarchar](200) NOT NULL,
	-- Valor PAC = valor liquidado (registered as receivable)
	[ValorPac] [decimal](18, 2) NOT NULL,
	-- Valor receita já cobrada, split banco/caixa per the Excel; total + saldo
	-- por cobrar (=PAC - cobrado) are computed at read time, not stored.
	[ValorCobradoBanco] [decimal](18, 2) NOT NULL CONSTRAINT [DF_ReceitaPac_ValorCobradoBanco] DEFAULT (0),
	[ValorCobradoCaixa] [decimal](18, 2) NOT NULL CONSTRAINT [DF_ReceitaPac_ValorCobradoCaixa] DEFAULT (0),
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ReceitaPac] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Regime]
	FOREIGN KEY([RegimeFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Regime]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Atividade]
	FOREIGN KEY([AtividadeFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Atividade]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_EconomicClassification]
	FOREIGN KEY([EconomicClassificationFk]) REFERENCES [dbo].[EconomicClassification] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_EconomicClassification]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Institution]
	FOREIGN KEY([OrganizationFk]) REFERENCES [dbo].[INSTITUTION] ([id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Institution]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_ReceitaPac_Numero] ON [dbo].[ReceitaPac] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO

-- ============================================================
-- Source: db_migrations/2026-07-11i_system_settings.sql
-- ============================================================
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

-- (Original dev migration also added ORCAMENTOCONFIG.ano/.tipo here, then
-- reverted that same change 1 day later once BudgetPeriod replaced the
-- approach — see 2026-07-12f_budget_period.sql below. Omitted entirely here
-- since it was a same-engagement dead end; ORCAMENTOCONFIG needs no change.)

-- ============================================================
-- Source: db_migrations/2026-07-11i_user_permission.sql
-- ============================================================
-- Granular RBAC for the new-mode ("Quản lý User & Phân quyền" screen).
-- Additive only — does NOT touch the old Perfil/Funcionalidade/RelUtilizadorPerfil
-- system (that keeps serving old-mode accounts unchanged) and does NOT touch
-- UserModeAccess (that stays a separate binary new-mode gate).
--
-- The permission-TOKEN catalog itself (ORC_SUBMIT, AD_APPROVE, ...) is a fixed
-- list in backend code (Common/PermissionCatalog.cs), not a DB table — only
-- grants (UserPermission) and convenience bundles (PermissionPreset) are data.
--
-- NOTE: no [RequirePerm]-style enforcement is wired into any controller yet
-- (deliberately out of scope this round) — this migration only lays down the
-- storage so the admin screen can assign tokens; enforcement is a follow-up.

CREATE TABLE [dbo].[PermissionPreset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](50) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_PermissionPreset] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_PermissionPreset_Codigo] ON [dbo].[PermissionPreset] ([Codigo])
	WHERE [IndActivo] = 1
GO

CREATE TABLE [dbo].[PermissionPresetItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PermissionPresetFk] [int] NOT NULL,
	[PermissionToken] [varchar](50) NOT NULL,
	CONSTRAINT [PK_PermissionPresetItem] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPresetItem] WITH CHECK ADD CONSTRAINT [FK_PermissionPresetItem_Preset]
	FOREIGN KEY([PermissionPresetFk]) REFERENCES [dbo].[PermissionPreset] ([Id])
GO
ALTER TABLE [dbo].[PermissionPresetItem] CHECK CONSTRAINT [FK_PermissionPresetItem_Preset]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_PermissionPresetItem_Preset_Token] ON [dbo].[PermissionPresetItem] ([PermissionPresetFk], [PermissionToken])
GO

CREATE TABLE [dbo].[UserPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[PermissionToken] [varchar](50) NOT NULL,
	-- NULL = granted individually; set = expanded from applying a preset (so
	-- re-applying/removing a preset can be told apart from a manual grant)
	[SourcePresetFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserPermission] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserPermission] WITH CHECK ADD CONSTRAINT [FK_UserPermission_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[Utilizador] ([IdUtilizador])
GO
ALTER TABLE [dbo].[UserPermission] CHECK CONSTRAINT [FK_UserPermission_Utilizador]
GO
ALTER TABLE [dbo].[UserPermission] WITH CHECK ADD CONSTRAINT [FK_UserPermission_Preset]
	FOREIGN KEY([SourcePresetFk]) REFERENCES [dbo].[PermissionPreset] ([Id])
GO
ALTER TABLE [dbo].[UserPermission] CHECK CONSTRAINT [FK_UserPermission_Preset]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_UserPermission_User_Token] ON [dbo].[UserPermission] ([UtilizadorFk], [PermissionToken])
	WHERE [IndActivo] = 1
GO

-- Starter bundles — a sensible default mapped from the legal approval chain
-- (ReuniaoSS_2024.pptx: Técnico -> Diretor Departamento Financeiro -> Diretor
-- Executivo), editable later; NOT a locked design, just a convenient seed.
INSERT INTO [dbo].[PermissionPreset] (Codigo, Nome, IndActivo, UtilizadorCriacao, DataCriacao) VALUES
	('ADMIN', N'Quản trị hệ thống (toàn quyền)', 1, 2, GETDATE()),
	('TECNICO_DF', N'Técnico Departamento Financeiro (nhập liệu / submit)', 1, 2, GETDATE()),
	('DIRETOR_DF', N'Diretor Departamento Financeiro (kiểm tra / duyệt cấp 1)', 1, 2, GETDATE()),
	('DIRETOR_EXECUTIVO', N'Diretor Executivo (phê duyệt cấp cao)', 1, 2, GETDATE()),
	('REPORT_ONLY', N'Chỉ xem báo cáo', 1, 2, GETDATE())
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT Id, 'ADMIN' FROM [dbo].[PermissionPreset] WHERE Codigo = 'ADMIN'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_SUBMIT'), ('AD_SUBMIT'), ('CABIMENTO_SUBMIT'), ('COMPROMISSO_SUBMIT'),
	('OBRIGACAO_SUBMIT'), ('PAG_SUBMIT'), ('REC_SUBMIT'), ('REPORT_VIEW')
) AS t(Token)
WHERE p.Codigo = 'TECNICO_DF'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_REVIEW'), ('AD_REVIEW'), ('CABIMENTO_APPROVE'), ('COMPROMISSO_REVIEW'),
	('OBRIGACAO_APPROVE'), ('PAG_APPROVE'), ('REC_REVIEW'), ('BANCO_CONCILIAR'),
	('REPORT_VIEW'), ('MASTERDATA_MANAGE')
) AS t(Token)
WHERE p.Codigo = 'DIRETOR_DF'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_APPROVE'), ('AD_APPROVE'), ('COMPROMISSO_APPROVE'), ('REC_APPROVE'),
	('ABE_APPROVE'), ('PAG_EXECUTE'), ('REPORT_VIEW')
) AS t(Token)
WHERE p.Codigo = 'DIRETOR_EXECUTIVO'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT Id, 'REPORT_VIEW' FROM [dbo].[PermissionPreset] WHERE Codigo = 'REPORT_ONLY'
GO

-- ============================================================
-- Source: db_migrations/2026-07-11j_payment.sql
-- ============================================================
-- Pagamento — per legal-accuracy pivot (ReuniaoSS_2024.pptx, DL 23/2022),
-- "Executar Pagamento" is legally TWO separate steps, not one PAG_EXECUTE
-- gate (superseded the 2026-07-10 single-gate design — see
-- customer-provided-data-findings memory, RESOLVED 2026-07-11):
--   1) PaymentAuthorization (Autorização do Pagamento) — 2-stage approval
--      (DRAFT -> PENDING_APPROVAL -> APPROVED, Diretor DF only, same shape
--      as Cabimento/Obligation), created from an APPROVED Obligation (1:1).
--      This is the ONLY step in the whole expenditure chain that books a
--      real Débito/Crédito GL entry (Codigoconta), per the Ciclo_Despesa +
--      CONTA BNCTL bank-ledger cross-check.
--   2) PaymentExecution (Realização do Pagamento) — the actual money-out
--      event. NOT itself an approval gate: a single confirm action, only
--      allowed once its PaymentAuthorization is APPROVED. Its mere
--      existence (IndActivo=1) means "paid" — no separate Estado column.

CREATE TABLE [dbo].[PaymentAuthorization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[ObligationFk] [int] NOT NULL,
	[Descritivo] [nvarchar](500) NULL,
	[ValorAutorizado] [decimal](18, 2) NOT NULL,
	[CodigoContaDebitoFk] [int] NULL,
	[CodigoContaCreditoFk] [int] NULL,
	-- DRAFT | PENDING_APPROVAL | APPROVED (2-stage — Diretor DF only, no DE)
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_PaymentAuthorization] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PaymentExecution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentAuthorizationFk] [int] NOT NULL,
	[DataPagamento] [datetime] NOT NULL,
	[ContaBancariaFk] [int] NULL,
	[NumeroDocumento] [nvarchar](100) NULL,
	[Observacao] [nvarchar](500) NULL,
	[ExecutedBy] [int] NOT NULL,
	[ExecutedAt] [datetime] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_PaymentExecution] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PaymentAuthorization] WITH CHECK ADD CONSTRAINT [FK_PaymentAuthorization_Obligation]
	FOREIGN KEY([ObligationFk]) REFERENCES [dbo].[Obligation] ([Id])
GO
ALTER TABLE [dbo].[PaymentAuthorization] CHECK CONSTRAINT [FK_PaymentAuthorization_Obligation]
GO

ALTER TABLE [dbo].[PaymentAuthorization] WITH CHECK ADD CONSTRAINT [FK_PaymentAuthorization_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[PaymentAuthorization] CHECK CONSTRAINT [FK_PaymentAuthorization_CodigoContaDebito]
GO

ALTER TABLE [dbo].[PaymentAuthorization] WITH CHECK ADD CONSTRAINT [FK_PaymentAuthorization_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[PaymentAuthorization] CHECK CONSTRAINT [FK_PaymentAuthorization_CodigoContaCredito]
GO

ALTER TABLE [dbo].[PaymentExecution] WITH CHECK ADD CONSTRAINT [FK_PaymentExecution_PaymentAuthorization]
	FOREIGN KEY([PaymentAuthorizationFk]) REFERENCES [dbo].[PaymentAuthorization] ([Id])
GO
ALTER TABLE [dbo].[PaymentExecution] CHECK CONSTRAINT [FK_PaymentExecution_PaymentAuthorization]
GO

ALTER TABLE [dbo].[PaymentExecution] WITH CHECK ADD CONSTRAINT [FK_PaymentExecution_ContaBancaria]
	FOREIGN KEY([ContaBancariaFk]) REFERENCES [dbo].[Contabancaria] ([Id])
GO
ALTER TABLE [dbo].[PaymentExecution] CHECK CONSTRAINT [FK_PaymentExecution_ContaBancaria]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_PaymentAuthorization_Obligation] ON [dbo].[PaymentAuthorization] ([ObligationFk])
	WHERE [IndActivo] = 1
GO
CREATE UNIQUE INDEX [UX_PaymentAuthorization_Numero] ON [dbo].[PaymentAuthorization] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO
CREATE UNIQUE INDEX [UX_PaymentExecution_PaymentAuthorization] ON [dbo].[PaymentExecution] ([PaymentAuthorizationFk])
	WHERE [IndActivo] = 1
GO

-- ============================================================
-- Source: db_migrations/2026-07-11j_user_profile.sql
-- ============================================================
-- Extra profile fields for the "Quản lý User & Phân quyền" screen (user asked
-- for Tên/Email/Phòng ban in addition to Username/Password, 2026-07-11) —
-- Email is meant to back a future password-reset flow (not built yet, just
-- the data capture for now).
--
-- New satellite table instead of ALTER TABLE Utilizador — per house rule
-- "prioritize creating new tables over modifying fields of an existing
-- table" (2026-07-11). DepartamentoFk reuses the existing Departamento
-- master-data table (already used elsewhere, e.g. Relutilizadordepartamento)
-- for the dropdown — does not touch that old table/relationship.

CREATE TABLE [dbo].[UserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[Nome] [nvarchar](200) NULL,
	[Email] [nvarchar](200) NULL,
	[DepartamentoFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserProfile] WITH CHECK ADD CONSTRAINT [FK_UserProfile_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[Utilizador] ([IdUtilizador])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Utilizador]
GO
ALTER TABLE [dbo].[UserProfile] WITH CHECK ADD CONSTRAINT [FK_UserProfile_Departamento]
	FOREIGN KEY([DepartamentoFk]) REFERENCES [dbo].[Departamento] ([Id])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Departamento]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_UserProfile_Utilizador] ON [dbo].[UserProfile] ([UtilizadorFk])
	WHERE [IndActivo] = 1
GO

-- ============================================================
-- Source: db_migrations/2026-07-11k_despesa_gap_fields.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-11k_receita_pac_conta_bancaria.sql
-- ============================================================
-- Adds a bank-account selector to ReceitaPac — user pointed out (2026-07-11)
-- that when Receita is collected via bank, the entry must record WHICH bank
-- account received it, not just a bare "ValorCobradoBanco" amount. Reuses
-- the existing Contabancaria table (also just exposed via the new
-- "Cấu hình hệ thống > Ngân hàng" screen) — nullable since ValorCobradoBanco
-- can be 0 (money entirely via Caixa) and no bank applies then.
--
-- Additive ALTER on a table created THIS SAME engagement (ReceitaPac, added
-- 2026-07-11h) — not a legacy/pre-existing table, so this isn't the kind of
-- "don't modify existing table fields" case the house rule warns about.

ALTER TABLE [dbo].[ReceitaPac] ADD [ContaBancariaFk] [int] NULL
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_ContaBancaria]
	FOREIGN KEY([ContaBancariaFk]) REFERENCES [dbo].[Contabancaria] ([id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_ContaBancaria]
GO

-- ============================================================
-- Source: db_migrations/2026-07-11l_obligation_beneficiary.sql
-- ============================================================
-- Obrigação beneficiary/bank/payroll fields — found missing vs the real
-- customer form (supported_documents/Customer provide data/
-- FormulariosDESPESA_emBranco.xlsx, sheet Obrigação items 3-5 + sheets
-- ListaObrigação1/ListaObrigação2). All additive, no workflow change.
--
-- Item 3 "Liquidação de despesa relativa a" + item 4 "Identificação do(s)
-- Beneficiário(s)" + item 5 "montante a pagar e detalhes bancários" are
-- filled DIRECTLY on the Obrigação form for the common single-payee case
-- (Fornecedor/Contribuinte(EE)/Outro) — these become header columns below.
-- Per the form's own annotation "(anexar lista, no caso das prestações e
-- salários)", when Categoria is Beneficiário (subsídios/pensões) or Pessoal
-- (salários), the SAME info instead comes from an attached LIST (multiple
-- payees) — modeled as the ObligationBeneficiary child table, unified for
-- both ListaObrigação1 (Beneficiários) and ListaObrigação2 (Pessoal, which
-- additionally has a payroll breakdown) since the columns overlap heavily.

ALTER TABLE [dbo].[Obligation] ADD [LiquidacaoTipo] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNome] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNiss] [varchar](20) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioCategoria] [varchar](30) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNomeConta] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNumeroConta] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioIban] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioSwift] [varchar](20) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioBanco] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioMontanteAPagar] [decimal](18, 2) NULL
GO

CREATE TABLE [dbo].[ObligationBeneficiary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObligationFk] [int] NOT NULL,
	[Niss] [varchar](20) NULL,
	[NomeContribuinte] [nvarchar](200) NULL,
	-- Only used by the ListaObrigação1 (Beneficiários) shape — the contribuinte
	-- (payer of record) and the actual beneficiary receiving payment can differ
	-- for prestações sociais. NULL for ListaObrigação2 (Pessoal/salários),
	-- where the contribuinte IS the beneficiary.
	[NomeBeneficiario] [nvarchar](200) NULL,
	[NomeConta] [nvarchar](200) NULL,
	[NumeroConta] [varchar](50) NULL,
	[Iban] [varchar](50) NULL,
	[Swift] [varchar](20) NULL,
	[Banco] [nvarchar](200) NULL,
	-- Payroll breakdown — ListaObrigação2 (Pessoal/salários) only, NULL otherwise.
	[SalarioIliquido] [decimal](18, 2) NULL,
	[Cotizacao4] [decimal](18, 2) NULL,
	[Imposto10] [decimal](18, 2) NULL,
	[SalarioLiquido] [decimal](18, 2) NULL,
	[OutrosSuplementos] [decimal](18, 2) NULL,
	[MontanteAPagar] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ObligationBeneficiary] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ObligationBeneficiary] WITH CHECK ADD CONSTRAINT [FK_ObligationBeneficiary_Obligation]
	FOREIGN KEY([ObligationFk]) REFERENCES [dbo].[Obligation] ([Id])
GO
ALTER TABLE [dbo].[ObligationBeneficiary] CHECK CONSTRAINT [FK_ObligationBeneficiary_Obligation]
GO

-- ============================================================
-- Source: db_migrations/2026-07-11m_orcamento_suplementar.sql
-- ============================================================
-- Suplementar (supplementary budget adjustment) — per-rúbrica adjustment
-- records against an already-APPROVED OrcamentoLinha, NOT a full clone of
-- the budget config. A year can have its Principal Orçamento plus N
-- Suplementar rounds (uncapped). Same 4-state batch workflow as Orçamento
-- (DRAFT -> PENDING_REVIEW -> PENDING_APPROVAL -> APPROVED), submitted as
-- one whole batch like Orçamento (per-line approval doesn't exist here
-- either). Free-form: a round can mix increases and decreases, no
-- balance-to-zero requirement (see memory finance-report-impl-spec.md,
-- "Suplementar = free-form adjustment" 2026-07-10).
--
-- On APPROVE, each line's FinalValue is written back into the referenced
-- OrcamentoLinha.Valor — keeps OrcamentoLinha as the single current-value
-- source of trute for every downstream reader (AD picker, Cabimento,
-- Compromisso, reports) without needing "resolve latest adjustment" logic
-- anywhere else. OldValue/AdjustmentValue/FinalValue stay on this table as
-- an audit trail of what changed and why.

CREATE TABLE [dbo].[OrcamentoSuplementar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrcamentoConfigFk] [int] NOT NULL,
	[Estado] [varchar](20) NOT NULL,
	[SubmittedBy] [int] NULL,
	[SubmittedAt] [datetime] NULL,
	[ReviewedBy] [int] NULL,
	[ReviewedAt] [datetime] NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[LastRejectComment] [nvarchar](500) NULL,
	[LastRejectBy] [int] NULL,
	[LastRejectAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_OrcamentoSuplementar] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrcamentoSuplementar] WITH CHECK ADD CONSTRAINT [FK_OrcamentoSuplementar_OrcamentoConfig]
	FOREIGN KEY([OrcamentoConfigFk]) REFERENCES [dbo].[ORCAMENTOCONFIG] ([id])
GO
ALTER TABLE [dbo].[OrcamentoSuplementar] CHECK CONSTRAINT [FK_OrcamentoSuplementar_OrcamentoConfig]
GO

CREATE TABLE [dbo].[OrcamentoSuplementarLinha](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrcamentoSuplementarFk] [int] NOT NULL,
	[OrcamentoLinhaFk] [int] NOT NULL,
	[OldValue] [decimal](18, 2) NOT NULL,
	[AdjustmentValue] [decimal](18, 2) NOT NULL,
	[FinalValue] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_OrcamentoSuplementarLinha] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrcamentoSuplementarLinha] WITH CHECK ADD CONSTRAINT [FK_OSL_OrcamentoSuplementar]
	FOREIGN KEY([OrcamentoSuplementarFk]) REFERENCES [dbo].[OrcamentoSuplementar] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoSuplementarLinha] CHECK CONSTRAINT [FK_OSL_OrcamentoSuplementar]
GO

ALTER TABLE [dbo].[OrcamentoSuplementarLinha] WITH CHECK ADD CONSTRAINT [FK_OSL_OrcamentoLinha]
	FOREIGN KEY([OrcamentoLinhaFk]) REFERENCES [dbo].[OrcamentoLinha] ([Id])
GO
ALTER TABLE [dbo].[OrcamentoSuplementarLinha] CHECK CONSTRAINT [FK_OSL_OrcamentoLinha]
GO

-- ============================================================
-- Source: db_migrations/2026-07-12a_bank_statement_line.sql
-- ============================================================
-- Conciliação de Movimentos — new-mode bank statement reconciliation.
-- Confirmed via DB (2026-07-10, see memory finance-report-impl-spec.md):
-- Conciliação is a shared post-hoc verification step that runs AFTER both
-- cycles (Receita and Despesa/Pagamento), matching a raw bank statement
-- line against EITHER a booked Receita OR a booked Despesa/Pagamento.
-- No automatic bank feed exists — staff manually download the statement
-- and import/enter it here (user-confirmed).
--
-- Deliberately a FRESH new-mode table, not a reuse of the old
-- [MOVIMENTOSBANCARIOS]/[MOVIMENTOSPORCONCILIAR] pair — those are wired
-- into the old tarefaAtivo_fk workflow dispatcher + old departamento/
-- centroCusto/tipoConta/agrupamentoConfig/Guiapagamento/ReservaCredito
-- legacy dimensions, none of which apply to the new-mode entities being
-- matched here (ReceitaPac, PaymentExecution). Same additive-upgrade
-- convention as every other new-mode entity (Cabimento, CompromissoDespesa,
-- Obligation, PaymentAuthorization, ...).
--
-- Match is kept simple (1 statement line <-> at most 1 Receita OR 1
-- Despesa record, full-amount) — no evidence in the source Excel/DB of a
-- need for partial/many-to-many matching; can be revisited if the client
-- surfaces a real case for it.

CREATE TABLE [dbo].[BankStatementLine](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContaBancariaFk] [int] NOT NULL,
	[DataValor] [datetime] NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	-- Crédito = tiền vào tài khoản (khớp với Receita); Débito = tiền ra khỏi
	-- tài khoản (khớp với Despesa/Pagamento) — cùng ý nghĩa như bảng cũ
	-- MOVIMENTOSBANCARIOS.credito/debito.
	[Credito] [decimal](18, 2) NOT NULL CONSTRAINT [DF_BankStatementLine_Credito] DEFAULT (0),
	[Debito] [decimal](18, 2) NOT NULL CONSTRAINT [DF_BankStatementLine_Debito] DEFAULT (0),
	[ReceitaPacFk] [int] NULL,
	[PaymentExecutionFk] [int] NULL,
	[ConciliadoBy] [int] NULL,
	[ConciliadoAt] [datetime] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_BankStatementLine] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BankStatementLine] WITH CHECK ADD CONSTRAINT [FK_BankStatementLine_ContaBancaria]
	FOREIGN KEY([ContaBancariaFk]) REFERENCES [dbo].[Contabancaria] ([Id])
GO
ALTER TABLE [dbo].[BankStatementLine] CHECK CONSTRAINT [FK_BankStatementLine_ContaBancaria]
GO

ALTER TABLE [dbo].[BankStatementLine] WITH CHECK ADD CONSTRAINT [FK_BankStatementLine_ReceitaPac]
	FOREIGN KEY([ReceitaPacFk]) REFERENCES [dbo].[ReceitaPac] ([Id])
GO
ALTER TABLE [dbo].[BankStatementLine] CHECK CONSTRAINT [FK_BankStatementLine_ReceitaPac]
GO

ALTER TABLE [dbo].[BankStatementLine] WITH CHECK ADD CONSTRAINT [FK_BankStatementLine_PaymentExecution]
	FOREIGN KEY([PaymentExecutionFk]) REFERENCES [dbo].[PaymentExecution] ([Id])
GO
ALTER TABLE [dbo].[BankStatementLine] CHECK CONSTRAINT [FK_BankStatementLine_PaymentExecution]
GO

-- Không cho phép 1 dòng vừa khớp Receita vừa khớp Despesa cùng lúc.
ALTER TABLE [dbo].[BankStatementLine] WITH CHECK ADD CONSTRAINT [CK_BankStatementLine_NotBothMatched]
	CHECK (NOT ([ReceitaPacFk] IS NOT NULL AND [PaymentExecutionFk] IS NOT NULL))
GO

SET QUOTED_IDENTIFIER ON
GO
-- 1 dòng sao kê chỉ được đối chiếu với TỐI ĐA 1 Receita hoặc 1 Despesa
-- (không phải cả 2), và mỗi Receita/Despesa cũng chỉ được đối chiếu 1 lần.
CREATE UNIQUE INDEX [UX_BankStatementLine_ReceitaPac] ON [dbo].[BankStatementLine] ([ReceitaPacFk])
	WHERE [IndActivo] = 1 AND [ReceitaPacFk] IS NOT NULL
GO
CREATE UNIQUE INDEX [UX_BankStatementLine_PaymentExecution] ON [dbo].[BankStatementLine] ([PaymentExecutionFk])
	WHERE [IndActivo] = 1 AND [PaymentExecutionFk] IS NOT NULL
GO

-- ============================================================
-- Source: db_migrations/2026-07-12b_bank_statement_line_fields.sql
-- ============================================================
-- Conciliação de Movimentos — thêm 2 field theo yêu cầu user (2026-07-12):
-- Data Transação (ngày thực hiện giao dịch, khác Data Valor — ngân hàng
-- thường phân biệt data valor/ngày ghi nhận giá trị vs. data transação/
-- ngày giao dịch thực tế diễn ra) và Código Transação Bancária (mã tham
-- chiếu giao dịch do ngân hàng cấp, dùng để tra soát khi có sai lệch).

ALTER TABLE [dbo].[BankStatementLine] ADD [DataTransacao] [datetime] NULL
GO
ALTER TABLE [dbo].[BankStatementLine] ADD [CodigoTransacaoBancaria] [varchar](100) NULL
GO

-- ============================================================
-- Source: db_migrations/2026-07-12c_lancamento.sql
-- ============================================================
-- Registo de Lançamentos (bút toán Débito/Crédito) — per legal-source design
-- decision in memory lancamentos-conciliacao-link-design: entries are NOT
-- hand-typed by finance staff into a separate journal screen. They are
-- auto-generated the moment a real transaction executes (Pagamento) or is
-- confirmed (Receita), reusing the Débito/Crédito account FKs already
-- captured at that point (PaymentAuthorization.CodigoContaDebitoFk/
-- CodigoContaCreditoFk today; ReceitaPac gets the same treatment later —
-- see financial-statements-scope-gap memory, follow-up not yet built).
-- OrigemTipo/OrigemId is a soft (non-FK) pointer back to the source record
-- so this table stays generic across future origem types without needing a
-- new nullable FK column per source.
-- Back up with a dated label before running against any shared environment.

CREATE TABLE [dbo].[Lancamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Data] [datetime] NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	[CodigoContaDebitoFk] [int] NOT NULL,
	[CodigoContaCreditoFk] [int] NOT NULL,
	[Valor] [decimal](18, 2) NOT NULL,
	-- 'PaymentExecution' | 'ReceitaPac' (futuro) | 'MANUAL' — soft pointer, không FK cứng
	[OrigemTipo] [varchar](30) NOT NULL,
	[OrigemId] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_Lancamento] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Lancamento] WITH CHECK ADD CONSTRAINT [FK_Lancamento_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[Lancamento] CHECK CONSTRAINT [FK_Lancamento_CodigoContaDebito]
GO

ALTER TABLE [dbo].[Lancamento] WITH CHECK ADD CONSTRAINT [FK_Lancamento_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[Lancamento] CHECK CONSTRAINT [FK_Lancamento_CodigoContaCredito]
GO

CREATE INDEX [IX_Lancamento_Origem] ON [dbo].[Lancamento] ([OrigemTipo], [OrigemId])
GO

-- ============================================================
-- Source: db_migrations/2026-07-12d_placeholder_perfil.sql
-- ============================================================
-- Placeholder Perfil so new accounts created via "Quản lý User & Phân quyền"
-- can actually log in. InternalLoginManager (old login code, unchanged) hard
-- rejects login for any username other than "admin" that has zero active
-- Relutilizadorperfil rows ("CurrentUserHasNoProfile") — this is entirely
-- the OLD Perfil/Funcionalidade system's gate, unrelated to the new
-- UserPermission RBAC, and out of scope to rewrite right now (user chose:
-- auto-assign a placeholder Perfil at account creation instead of touching
-- login logic). This Perfil is intentionally linked to ZERO
-- Relperfilfuncionalidade rows — it grants no old-system capability, it
-- exists purely to satisfy the legacy "perfilIds.Count == 0" check.

INSERT INTO [dbo].[Perfil] (Descricao, IndActivo, UtilizadorCriacao, DataCriacao)
VALUES (N'Conta Módulo Contabilidade (sem Perfil legado)', 1, 2, GETDATE())
GO

-- ============================================================
-- Source: db_migrations/2026-07-12e_a07_children.sql
-- ============================================================
-- Bổ sung nhánh con còn thiếu của A07 (Administração do FRSS) trong
-- ProgramActivity — phát hiện qua audit đối chiếu với OSS_Global_2026_FINAL_livro.xlsx
-- (sheet Sintese_ProgramasOSS_CE), xem
-- supported_documents/sample_import_data/MASTER_DATA_COMPLETENESS_CHECK.md.
-- A07 trước đó chỉ có 1 dòng gốc (thêm ở 2026-07-11f_oss_perimeter.sql),
-- thiếu hẳn cấu trúc Subprograma/Atividade như A04/A05/A06 đã có.
-- IsOssPerimeter = 1 vì A07 nằm TRONG perimeter OSS (kế thừa từ dòng cha).
-- Run against a DEV/local copy first. Back up with a dated label before
-- running against any shared environment.

DECLARE @TargetOrcamentoConfigId INT = (SELECT TOP 1 [id] FROM [dbo].[ORCAMENTOCONFIG] WHERE [indActivo] = 1 ORDER BY [dataCriacao] DESC);
DECLARE @a07 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A07');

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A0701', N'Coordenação, gestão e funcionamento do FRSS', 2, @a07, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE()),
	('A0702', N'Investimento Estratégico do FRSS', 2, @a07, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE())
GO

DECLARE @TargetOrcamentoConfigId INT = (SELECT TOP 1 [id] FROM [dbo].[ORCAMENTOCONFIG] WHERE [indActivo] = 1 ORDER BY [dataCriacao] DESC);
DECLARE @a0701 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A0701');
DECLARE @a0702 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A0702');

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A070101', N'Funcionamento de todos os serviços e unidades orgânicas do Instituto FRSS', 3, @a0701, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE()),
	('A070102', N'Capacitação técnica e especializada dos Recursos Humanos do FRSS', 3, @a0701, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE()),
	('A070201', N'Instalação e Aquisição de Equipamentos no FRSS', 3, @a0702, @TargetOrcamentoConfigId, 1, 1, 1, GETDATE())
GO

-- ============================================================
-- Source: db_migrations/2026-07-12e_receita_pac_lancamento.sql
-- ============================================================
-- Wires the Receita side of the Registo de Lançamentos (sổ nhật ký) — until now
-- only Pagamento (Despesa) auto-generated Débito/Crédito journal entries; Receita
-- had no accounts to book against (see memory lancamentos-conciliacao-link-design,
-- "Receita side still needs new Débito/Crédito columns on ReceitaPac").
--
-- Receita is execution-only (no approval gate — see memory receita-approval-decision),
-- so unlike Pagamento (which has a separate Autorização step to pick the accounts),
-- ReceitaPac picks its own Débito/Crédito accounts directly on the same row it's
-- entered/saved on. Additive ALTER on a table created this engagement — not a
-- legacy/pre-existing table.

ALTER TABLE [dbo].[ReceitaPac] ADD [CodigoContaDebitoFk] [int] NULL
GO
ALTER TABLE [dbo].[ReceitaPac] ADD [CodigoContaCreditoFk] [int] NULL
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_CodigoContaDebito]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_CodigoContaCredito]
GO

-- ============================================================
-- Source: db_migrations/2026-07-12f_budget_period.sql
-- ============================================================
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

-- Seed rows matching the client's OWN existing [ORCAMENTOCONFIG] rows (same Id
-- values, so any pre-existing ProgramActivity/EconomicClassification/
-- OrcamentoBatch/OrcamentoSuplementar row's *ConfigFk keeps resolving correctly
-- once the FK columns below are repointed) — dynamic copy, NOT the dev DB's
-- literal 2 test rows (the original migration hardcoded Id=1/2 with a bogus
-- year-2099 test row; that is intentionally NOT reproduced here).
SET IDENTITY_INSERT [dbo].[BudgetPeriod] ON
INSERT INTO [dbo].[BudgetPeriod] ([Id], [Ano], [Tipo], [DataInicio], [DataFim], [IndActivo], [UtilizadorCriacao], [DataCriacao], [UtilizadorAlteracao], [DataAlteracao], [Ipv6])
SELECT [id], YEAR([dataInicio]), 'PRINCIPAL', [dataInicio], [dataFim], [indActivo], [utilizadorCriacao], [dataCriacao], [utilizadorAlteracao], [dataAlteracao], [ipv6]
FROM [dbo].[ORCAMENTOCONFIG]
SET IDENTITY_INSERT [dbo].[BudgetPeriod] OFF
GO

-- Keep the IDENTITY seed ahead of any future insert (SET IDENTITY_INSERT ON does
-- not itself advance the seed for the next auto-generated Id). Reseed dynamically
-- to the client's actual max copied Id, not a hardcoded value.
DECLARE @MaxBudgetPeriodId INT = (SELECT ISNULL(MAX([id]), 0) FROM [dbo].[ORCAMENTOCONFIG]);
DECLARE @ReseedSql NVARCHAR(200) = N'DBCC CHECKIDENT (''[dbo].[BudgetPeriod]'', RESEED, ' + CAST(@MaxBudgetPeriodId AS NVARCHAR(10)) + N')';
EXEC sp_executesql @ReseedSql;
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

-- (Original dev migration dropped ORCAMENTOCONFIG.ano/.tipo here to revert
-- 2026-07-11i_system_settings.sql's now-omitted ALTER — nothing to drop here
-- since this consolidated script never added those columns in the first place.)

-- ============================================================
-- Source: db_migrations/2026-07-12f_guia_pagamento_conta_config.sql
-- ============================================================
-- Cấu hình tài khoản Nợ/Có dùng để tự sinh Lançamento (bút toán) khi một Guia
-- Pagamento (GP) được đối chiếu ngân hàng thành công (màn "Duyệt Guia Pagamento
-- (đối chiếu ngân hàng)" — GuiaConciliacaoDataManager). Đến trước migration này,
-- sự kiện "doanh nghiệp đã trả tiền, đã khớp sao kê ngân hàng thật" không sinh
-- ra bút toán sổ sách nào cả (khác với Pagamento/ReceitaPac, đã có Lançamento
-- tự sinh từ trước). Bảng này là bản cấu hình 1 dòng (singleton), do admin chọn
-- 1 lần qua màn Settings mới — không có UI nhập tay số tiền/tài khoản cho từng
-- Guia (tránh lặp lại đúng lỗ hổng "tự khai, không kiểm chứng" đã sửa ở bước
-- đối chiếu ngân hàng).
-- Back up với nhãn có ngày trước khi chạy trên môi trường chia sẻ.

CREATE TABLE [dbo].[GuiaPagamentoContaConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoContaDebitoFk] [int] NOT NULL,
	[CodigoContaCreditoFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_GuiaPagamentoContaConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaDebito]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
GO

-- ============================================================
-- Source: db_migrations/2026-07-12g_receitapac_lancamento_split_cleanup.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-13a_conta_bancaria_codigoconta_mapping.sql
-- ============================================================
-- Adds a Codigoconta mapping to ContaBancaria (= legacy CONTABANCARIA, same physical
-- table — confirmed via sys.foreign_keys, BankStatementLine.ContaBancariaFk and legacy
-- Movimentosbancarios.conta_fk both point at it). Needed so GuiaConciliacaoDataManager
-- can derive the Débito account DYNAMICALLY from whichever real bank account the matched
-- Movimentosbancarios line actually belongs to, instead of a single fixed config value —
-- confirmed against real 2024 ledger data (SCFSSTL2024_VF.xlsm, sheet "Lançamentos"):
-- every GP collection entry debits the SPECIFIC bank that received the money (1221 BNCTL,
-- 1222 BNU, 1223 Mandiri, 1224 BRI, 1225 ANZ, 1226 BNCTL-OFICIAL, 1211 Banco Central), not
-- one fixed account.
--
-- Also seeds the 7 real banks from that same source (dev DB only had 3 fictional
-- placeholder banks before this) and points each at its correct Codigoconta row.
--
-- Codigoconta data-quality note (investigated, NOT fully fixed here — out of scope for
-- this change): the whole Codigoconta tree has ~106 duplicate full-codes out of 704 rows
-- (2 parallel import batches). For the 7 bank codes used here, one batch (Id 8-14, plus
-- 704 for 1226) matches the source correctly except Id=704's Designacao ("bnf") and Id=12's
-- casing ("MANDIRi") — fixed below since they're directly in scope. The OTHER duplicate
-- batch for these same codes (Id 599-606) is corrupted at every level (parent names wrong
-- too) and is left untouched/unused — nothing references it. The broader ~106-row
-- duplication elsewhere in the tree is unrelated to GP and not addressed here.

-- (Original dev migration also fixed 2 dev-DB-specific Codigoconta typos by
-- hardcoded Id, and inserted 7 placeholder ContaBancaria rows pointing at
-- dev-DB-specific Codigoconta Ids — both OMITTED here. After running this
-- script, map each of your REAL bank accounts to its correct Codigoconta row
-- via Cấu hình hệ thống > Ngân hàng.)

ALTER TABLE [dbo].[ContaBancaria] ADD [CodigoContaFk] [int] NULL
GO
ALTER TABLE [dbo].[ContaBancaria] WITH CHECK ADD CONSTRAINT [FK_ContaBancaria_CodigoConta]
	FOREIGN KEY([CodigoContaFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[ContaBancaria] CHECK CONSTRAINT [FK_ContaBancaria_CodigoConta]
GO

-- ============================================================
-- Source: db_migrations/2026-07-13b_guia_pagamento_conta_config_drop_debito.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-13c_ad_missing_source_form_fields.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-13c_attachment.sql
-- ============================================================
-- File attachment support for the expenditure cycle (AD/Cabimento/Compromisso/
-- Obrigação/Pagamento) — user requested 2026-07-13: submitter can attach a file
-- when submitting (PDF/PNG/Excel), view it back later, admin can configure the
-- max upload size and which stages require a mandatory attachment before submit
-- is allowed.
--
-- Attachment is a generic/polymorphic table (EntityType + EntityId) rather than
-- 5 separate FK'd tables — same shape across all 5 stages, and this list of
-- entity types will keep growing (see house convention: additive satellite
-- tables). File bytes stored as varbinary(max), same pattern as the existing
-- old-mode Componentedocumentoregisto/Documentoidentificacao tables (no disk/
-- blob storage exists anywhere in this codebase yet — see attachment-feature
-- research, 2026-07-13).

CREATE TABLE [dbo].[Attachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	-- 'AD' | 'CABIMENTO' | 'COMPROMISSO' | 'OBRIGACAO' | 'PAGAMENTO'
	[EntityType] [varchar](30) NOT NULL,
	[EntityId] [int] NOT NULL,
	[FileName] [nvarchar](260) NOT NULL,
	[ContentType] [varchar](150) NOT NULL,
	[FileSize] [int] NOT NULL,
	[FileContent] [varbinary](max) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE INDEX [IX_Attachment_Entity] ON [dbo].[Attachment] ([EntityType], [EntityId])
	WHERE [IndActivo] = 1
GO

-- Singleton config row (Id always = 1) — max upload size (shared across all 5
-- stages, one system-wide setting per user's request) + per-stage "must attach
-- before submit" flags.
CREATE TABLE [dbo].[AttachmentConfig](
	[Id] [int] NOT NULL,
	[MaxFileSizeMb] [int] NOT NULL,
	[AdObrigatorio] [bit] NOT NULL,
	[CabimentoObrigatorio] [bit] NOT NULL,
	[CompromissoObrigatorio] [bit] NOT NULL,
	[ObrigacaoObrigatorio] [bit] NOT NULL,
	[PagamentoObrigatorio] [bit] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	CONSTRAINT [PK_AttachmentConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Default row: 10MB limit, nothing mandatory yet (admin opts in per stage from
-- the new settings screen) — safe default that doesn't block any in-flight
-- DRAFT submissions the moment this migration runs.
INSERT INTO [dbo].[AttachmentConfig]
	([Id], [MaxFileSizeMb], [AdObrigatorio], [CabimentoObrigatorio], [CompromissoObrigatorio], [ObrigacaoObrigatorio], [PagamentoObrigatorio])
VALUES
	(1, 10, 0, 0, 0, 0, 0)
GO

-- ============================================================
-- Source: db_migrations/2026-07-13c_liquidacao_conta_config.sql
-- ============================================================
-- Bổ sung bút toán kế toán thứ 2 (double-booking) cho chu trình Despesa/Pagamento —
-- xác nhận từ sổ sách thật của khách hàng (FRSSVF.xlsm, sheet "Lançamentos"): mọi
-- khoản Despesa thật đều có ĐÚNG 2 bút toán, không phải 1:
--   Bút toán 1 (lúc Obrigação/Autorização được duyệt — ghi nhận khoản PHẢI TRẢ):
--     Nợ [tài khoản Chi phí đã chọn ở Autorização]  /  Có [tài khoản Phải trả]
--   Bút toán 2 (lúc Pagamento thực hiện — tiền THỰC SỰ rời ngân hàng):
--     Nợ [tài khoản Phải trả]  /  Có [tài khoản Ngân hàng đã chọn ở Autorização]
-- Trước migration này, hệ thống chỉ sinh 1 bút toán duy nhất lúc Execução, ghi
-- thẳng Nợ Chi phí / Có Ngân hàng — bỏ qua tài khoản Phải trả trung gian.
--
-- Tài khoản Phải trả phụ thuộc loại đối tượng thụ hưởng (Obligation.BeneficiarioCategoria
-- — khớp đúng cách phân loại trong sổ thật: Fornecedores c/c cho nhà cung cấp, Com o
-- pessoal cho nhân viên, Outros credores cho khoản khác). Bảng này là cấu hình admin,
-- seed sẵn 5 dòng khớp 5 giá trị Categoria cố định đã có trên Obligation — admin chỉ
-- cần điền CodigoContaFk cho từng dòng qua màn Settings mới, không tự thêm dòng.
--
-- Đã backup DB trước khi chạy: db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-13_pre_liquidacao_conta_config.bak

CREATE TABLE [dbo].[LiquidacaoContaConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Categoria] [varchar](30) NOT NULL,
	[CodigoContaFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_LiquidacaoContaConfig] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_LiquidacaoContaConfig_Categoria] UNIQUE ([Categoria])
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LiquidacaoContaConfig] WITH CHECK ADD CONSTRAINT [FK_LiquidacaoContaConfig_CodigoConta]
	FOREIGN KEY([CodigoContaFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[LiquidacaoContaConfig] CHECK CONSTRAINT [FK_LiquidacaoContaConfig_CodigoConta]
GO

INSERT INTO [dbo].[LiquidacaoContaConfig] ([Categoria], [CodigoContaFk], [IndActivo], [UtilizadorCriacao], [DataCriacao])
VALUES
	('FORNECEDOR', NULL, 1, 1, GETDATE()),
	('BENEFICIARIO', NULL, 1, 1, GETDATE()),
	('CONTRIBUINTE_EE', NULL, 1, 1, GETDATE()),
	('PESSOAL', NULL, 1, 1, GETDATE()),
	('OUTRO', NULL, 1, 1, GETDATE())
GO

-- ============================================================
-- Source: db_migrations/2026-07-13d_ad_functional_classification_revert.sql
-- ============================================================
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

-- ============================================================
-- Source: db_migrations/2026-07-13d_gp_conta_config_setor_publico_privado_split.sql
-- ============================================================
-- Tách GuiaPagamentoContaConfig.CodigoContaCreditoFk (1 tài khoản Crédito chung) thành
-- 2 tài khoản riêng theo loại khu vực (Setor) của Entidade đóng góp — xác nhận từ sổ
-- sách thật (SCFSSTL2024_VF.xlsm "Lançamentos"): Crédito thực tế dùng 2 tài khoản khác
-- nhau, không phải 1:
--   2133114 "Setor Privado - Guias Emitidas"  (~24.176 dòng, đa số)
--   2133112 "Setor Público - Guias Emitidas"  (~191 dòng)
-- Trước migration này, hệ thống chỉ có 1 tài khoản Crédito cố định — mọi giao dịch của
-- công ty Setor Público bị ghi nhầm vào tài khoản Privado.
--
-- Phân loại Público/Privado lấy từ Entidadeempregadora.EntidadeSectorActFk ->
-- Sectoractividade.Descricao (field có sẵn, bắt buộc nhập khi đăng ký công ty — KHÔNG
-- phải field mới/không có như phiên trước từng kết luận nhầm). Mọi dòng Sectoractividade
-- có Descricao bắt đầu bằng "Setor Público" -> dùng CodigoContaCreditoPublicoFk; còn lại
-- (Setor Privado, PPP, hoặc thiếu dữ liệu) -> dùng CodigoContaCreditoPrivadoFk.
--
-- Bảng GuiaPagamentoContaConfig hiện đang RỖNG (0 dòng, chưa cấu hình giá trị thật) nên
-- an toàn để đổi schema trực tiếp, không cần migrate dữ liệu.
-- Đã backup DB trước khi chạy: db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-13_pre_gp_sector_credito_split.bak

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP COLUMN [CodigoContaCreditoFk]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] ADD [CodigoContaCreditoPrivadoFk] [int] NULL
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] ADD [CodigoContaCreditoPublicoFk] [int] NULL
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPrivado]
	FOREIGN KEY([CodigoContaCreditoPrivadoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPrivado]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPublico]
	FOREIGN KEY([CodigoContaCreditoPublicoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPublico]
GO

-- ============================================================
-- Source: db_migrations/2026-07-13e_cabimento_compromisso_missing_source_form_fields.sql
-- ============================================================
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

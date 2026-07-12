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

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

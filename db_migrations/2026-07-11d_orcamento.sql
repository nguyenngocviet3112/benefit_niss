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

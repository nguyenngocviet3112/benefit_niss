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

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

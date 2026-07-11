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

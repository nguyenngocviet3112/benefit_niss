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

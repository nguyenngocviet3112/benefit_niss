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

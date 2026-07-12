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

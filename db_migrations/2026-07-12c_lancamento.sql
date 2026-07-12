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

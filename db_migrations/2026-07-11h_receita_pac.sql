-- ReceitaPac — new-mode Receita entry (RECEITAS_PAC sheet: "REGISTO DE RECEITAS
-- LIQUIDADAS - OUTRAS" — bank interest, opening-balance carryover code 408,
-- etc). Does NOT cover RECEITAS_GP (contribution income, 401.xx) — that data
-- already lives in the Contribuições module (GuiaPagamento); Finance only
-- reconciles/reports it, no new entry screen needed for it.
--
-- Distinct from the OLD [COMPONENTERECEITA_REGISTO] table (workflow-driven
-- tarefaActivo_fk dispatcher, agrupamentoConfig_fk legacy tree, no separate
-- Crédito tracking) — this is a fresh satellite table against the NEW
-- EconomicClassification/ProgramActivity master data, per the additive-
-- upgrade convention used for Orcamento/ExpenditureAuthorization/Cabimento/
-- CompromissoDespesa.
--
-- EXECUTION-ONLY, no approval gate — unlike Despesa. Resolved 2026-07-11
-- after finding a contradiction in supported_documents/Customer provide
-- data/Primeira Fase - Doc. de especificações técnicas completo-
-- SRSD_Mod_Cont_Mod_Fin.pdf (p.43 says "Registo de receitas" with no
-- "Aprovação", vs the old shared Execução table schema implying a common
-- Cabimentado/Aprovado state machine for both Receita and Despesa) — user
-- confirmed execution-only, and separately said the DB-architecture PDF
-- (Modulo_Interno_Mod_Financeiro_Mod_Contribuicoes_Arquitectura_BD.pdf) is
-- outdated and should be disregarded. See memory receita-approval-decision.
--
-- Regime is NOT derived from Atividade here (differs from Despesa, confirmed
-- 2026-07-10 via real RECEITAS_PAC rows showing Regime populated with
-- Atividade blank) — RegimeFk is an independent FK straight to a ROOT
-- ProgramActivity row (A04/A05/A06/A07/A08), AtividadeFk is optional and can
-- be any level. Both point at ProgramActivity — two separate FKs to the same
-- table, same pattern already used elsewhere for dual references.

CREATE TABLE [dbo].[ReceitaPac](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[Ano] [int] NOT NULL,
	[Niss] [varchar](20) NULL,
	[RegimeFk] [int] NOT NULL,
	[AtividadeFk] [int] NULL,
	[EconomicClassificationFk] [int] NOT NULL,
	[OrganizationFk] [int] NOT NULL,
	[Descritivo] [nvarchar](200) NOT NULL,
	-- Valor PAC = valor liquidado (registered as receivable)
	[ValorPac] [decimal](18, 2) NOT NULL,
	-- Valor receita já cobrada, split banco/caixa per the Excel; total + saldo
	-- por cobrar (=PAC - cobrado) are computed at read time, not stored.
	[ValorCobradoBanco] [decimal](18, 2) NOT NULL CONSTRAINT [DF_ReceitaPac_ValorCobradoBanco] DEFAULT (0),
	[ValorCobradoCaixa] [decimal](18, 2) NOT NULL CONSTRAINT [DF_ReceitaPac_ValorCobradoCaixa] DEFAULT (0),
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ReceitaPac] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Regime]
	FOREIGN KEY([RegimeFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Regime]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Atividade]
	FOREIGN KEY([AtividadeFk]) REFERENCES [dbo].[ProgramActivity] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Atividade]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_EconomicClassification]
	FOREIGN KEY([EconomicClassificationFk]) REFERENCES [dbo].[EconomicClassification] ([Id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_EconomicClassification]
GO

ALTER TABLE [dbo].[ReceitaPac] WITH CHECK ADD CONSTRAINT [FK_ReceitaPac_Institution]
	FOREIGN KEY([OrganizationFk]) REFERENCES [dbo].[INSTITUTION] ([id])
GO
ALTER TABLE [dbo].[ReceitaPac] CHECK CONSTRAINT [FK_ReceitaPac_Institution]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_ReceitaPac_Numero] ON [dbo].[ReceitaPac] ([Numero], [Mes], [Ano])
	WHERE [IndActivo] = 1
GO

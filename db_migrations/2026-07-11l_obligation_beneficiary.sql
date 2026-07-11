-- Obrigação beneficiary/bank/payroll fields — found missing vs the real
-- customer form (supported_documents/Customer provide data/
-- FormulariosDESPESA_emBranco.xlsx, sheet Obrigação items 3-5 + sheets
-- ListaObrigação1/ListaObrigação2). All additive, no workflow change.
--
-- Item 3 "Liquidação de despesa relativa a" + item 4 "Identificação do(s)
-- Beneficiário(s)" + item 5 "montante a pagar e detalhes bancários" are
-- filled DIRECTLY on the Obrigação form for the common single-payee case
-- (Fornecedor/Contribuinte(EE)/Outro) — these become header columns below.
-- Per the form's own annotation "(anexar lista, no caso das prestações e
-- salários)", when Categoria is Beneficiário (subsídios/pensões) or Pessoal
-- (salários), the SAME info instead comes from an attached LIST (multiple
-- payees) — modeled as the ObligationBeneficiary child table, unified for
-- both ListaObrigação1 (Beneficiários) and ListaObrigação2 (Pessoal, which
-- additionally has a payroll breakdown) since the columns overlap heavily.

ALTER TABLE [dbo].[Obligation] ADD [LiquidacaoTipo] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNome] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNiss] [varchar](20) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioCategoria] [varchar](30) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNomeConta] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioNumeroConta] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioIban] [varchar](50) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioSwift] [varchar](20) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioBanco] [nvarchar](200) NULL
GO
ALTER TABLE [dbo].[Obligation] ADD [BeneficiarioMontanteAPagar] [decimal](18, 2) NULL
GO

CREATE TABLE [dbo].[ObligationBeneficiary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObligationFk] [int] NOT NULL,
	[Niss] [varchar](20) NULL,
	[NomeContribuinte] [nvarchar](200) NULL,
	-- Only used by the ListaObrigação1 (Beneficiários) shape — the contribuinte
	-- (payer of record) and the actual beneficiary receiving payment can differ
	-- for prestações sociais. NULL for ListaObrigação2 (Pessoal/salários),
	-- where the contribuinte IS the beneficiary.
	[NomeBeneficiario] [nvarchar](200) NULL,
	[NomeConta] [nvarchar](200) NULL,
	[NumeroConta] [varchar](50) NULL,
	[Iban] [varchar](50) NULL,
	[Swift] [varchar](20) NULL,
	[Banco] [nvarchar](200) NULL,
	-- Payroll breakdown — ListaObrigação2 (Pessoal/salários) only, NULL otherwise.
	[SalarioIliquido] [decimal](18, 2) NULL,
	[Cotizacao4] [decimal](18, 2) NULL,
	[Imposto10] [decimal](18, 2) NULL,
	[SalarioLiquido] [decimal](18, 2) NULL,
	[OutrosSuplementos] [decimal](18, 2) NULL,
	[MontanteAPagar] [decimal](18, 2) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_ObligationBeneficiary] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ObligationBeneficiary] WITH CHECK ADD CONSTRAINT [FK_ObligationBeneficiary_Obligation]
	FOREIGN KEY([ObligationFk]) REFERENCES [dbo].[Obligation] ([Id])
GO
ALTER TABLE [dbo].[ObligationBeneficiary] CHECK CONSTRAINT [FK_ObligationBeneficiary_Obligation]
GO

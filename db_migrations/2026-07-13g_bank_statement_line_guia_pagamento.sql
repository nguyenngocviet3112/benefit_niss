-- Bảng nối N-N giữa BankStatementLine (sao kê ngân hàng mode mới) và
-- Guiapagamento (mode cũ) — phần còn lại của việc hợp nhất mode mới sang
-- dùng hẳn BankStatementLine cho cả Guia Pagamento (trước đây đối chiếu
-- Guia Pagamento vẫn dùng bảng cũ MOVIMENTOSBANCARIOS/
-- REL_MOVIMENTOSPORCONCILIAR_MOVIMENTOS — xem memory
-- bank-statement-line-guia-pagamento-unification).
--
-- Khác với BankStatementLine.ReceitaPacFk/PaymentExecutionFk (1 dòng : 1
-- Receita/Pagamento, đủ dùng cho 2 luồng đó) — Guia Pagamento cần "1 dòng :
-- N Guia" hoặc "N dòng : 1 Guia" thật (khách xác nhận 2026-07-13, ví dụ
-- ngân hàng chuyển gộp nhiều Guia trong 1 giao dịch), nên phải dùng bảng
-- nối riêng thay vì thêm 1 cột FK đơn trên BankStatementLine.
--
-- Nhớ backup DB trước khi chạy migration này (theo quy ước
-- db_backup/dated_backups).

CREATE TABLE [dbo].[BankStatementLineGuiaPagamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankStatementLineFk] [int] NOT NULL,
	[GuiaPagamentoFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL CONSTRAINT [DF_BankStatementLineGuiaPagamento_IndActivo] DEFAULT (1),
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
 CONSTRAINT [PK_BankStatementLineGuiaPagamento] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BankStatementLineGuiaPagamento] WITH CHECK ADD CONSTRAINT [FK_BankStatementLineGuiaPagamento_BankStatementLine]
	FOREIGN KEY([BankStatementLineFk]) REFERENCES [dbo].[BankStatementLine] ([Id])
GO
ALTER TABLE [dbo].[BankStatementLineGuiaPagamento] CHECK CONSTRAINT [FK_BankStatementLineGuiaPagamento_BankStatementLine]
GO

ALTER TABLE [dbo].[BankStatementLineGuiaPagamento] WITH CHECK ADD CONSTRAINT [FK_BankStatementLineGuiaPagamento_Guiapagamento]
	FOREIGN KEY([GuiaPagamentoFk]) REFERENCES [dbo].[GUIAPAGAMENTO] ([idGuia])
GO
ALTER TABLE [dbo].[BankStatementLineGuiaPagamento] CHECK CONSTRAINT [FK_BankStatementLineGuiaPagamento_Guiapagamento]
GO

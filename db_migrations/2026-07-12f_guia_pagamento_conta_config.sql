-- Cấu hình tài khoản Nợ/Có dùng để tự sinh Lançamento (bút toán) khi một Guia
-- Pagamento (GP) được đối chiếu ngân hàng thành công (màn "Duyệt Guia Pagamento
-- (đối chiếu ngân hàng)" — GuiaConciliacaoDataManager). Đến trước migration này,
-- sự kiện "doanh nghiệp đã trả tiền, đã khớp sao kê ngân hàng thật" không sinh
-- ra bút toán sổ sách nào cả (khác với Pagamento/ReceitaPac, đã có Lançamento
-- tự sinh từ trước). Bảng này là bản cấu hình 1 dòng (singleton), do admin chọn
-- 1 lần qua màn Settings mới — không có UI nhập tay số tiền/tài khoản cho từng
-- Guia (tránh lặp lại đúng lỗ hổng "tự khai, không kiểm chứng" đã sửa ở bước
-- đối chiếu ngân hàng).
-- Back up với nhãn có ngày trước khi chạy trên môi trường chia sẻ.

CREATE TABLE [dbo].[GuiaPagamentoContaConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoContaDebitoFk] [int] NOT NULL,
	[CodigoContaCreditoFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_GuiaPagamentoContaConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaDebito]
	FOREIGN KEY([CodigoContaDebitoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaDebito]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
	FOREIGN KEY([CodigoContaCreditoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
GO

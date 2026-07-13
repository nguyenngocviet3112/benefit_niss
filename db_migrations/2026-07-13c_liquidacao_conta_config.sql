-- Bổ sung bút toán kế toán thứ 2 (double-booking) cho chu trình Despesa/Pagamento —
-- xác nhận từ sổ sách thật của khách hàng (FRSSVF.xlsm, sheet "Lançamentos"): mọi
-- khoản Despesa thật đều có ĐÚNG 2 bút toán, không phải 1:
--   Bút toán 1 (lúc Obrigação/Autorização được duyệt — ghi nhận khoản PHẢI TRẢ):
--     Nợ [tài khoản Chi phí đã chọn ở Autorização]  /  Có [tài khoản Phải trả]
--   Bút toán 2 (lúc Pagamento thực hiện — tiền THỰC SỰ rời ngân hàng):
--     Nợ [tài khoản Phải trả]  /  Có [tài khoản Ngân hàng đã chọn ở Autorização]
-- Trước migration này, hệ thống chỉ sinh 1 bút toán duy nhất lúc Execução, ghi
-- thẳng Nợ Chi phí / Có Ngân hàng — bỏ qua tài khoản Phải trả trung gian.
--
-- Tài khoản Phải trả phụ thuộc loại đối tượng thụ hưởng (Obligation.BeneficiarioCategoria
-- — khớp đúng cách phân loại trong sổ thật: Fornecedores c/c cho nhà cung cấp, Com o
-- pessoal cho nhân viên, Outros credores cho khoản khác). Bảng này là cấu hình admin,
-- seed sẵn 5 dòng khớp 5 giá trị Categoria cố định đã có trên Obligation — admin chỉ
-- cần điền CodigoContaFk cho từng dòng qua màn Settings mới, không tự thêm dòng.
--
-- Đã backup DB trước khi chạy: db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-13_pre_liquidacao_conta_config.bak

CREATE TABLE [dbo].[LiquidacaoContaConfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Categoria] [varchar](30) NOT NULL,
	[CodigoContaFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_LiquidacaoContaConfig] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_LiquidacaoContaConfig_Categoria] UNIQUE ([Categoria])
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LiquidacaoContaConfig] WITH CHECK ADD CONSTRAINT [FK_LiquidacaoContaConfig_CodigoConta]
	FOREIGN KEY([CodigoContaFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[LiquidacaoContaConfig] CHECK CONSTRAINT [FK_LiquidacaoContaConfig_CodigoConta]
GO

INSERT INTO [dbo].[LiquidacaoContaConfig] ([Categoria], [CodigoContaFk], [IndActivo], [UtilizadorCriacao], [DataCriacao])
VALUES
	('FORNECEDOR', NULL, 1, 1, GETDATE()),
	('BENEFICIARIO', NULL, 1, 1, GETDATE()),
	('CONTRIBUINTE_EE', NULL, 1, 1, GETDATE()),
	('PESSOAL', NULL, 1, 1, GETDATE()),
	('OUTRO', NULL, 1, 1, GETDATE())
GO

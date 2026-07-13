-- Tách GuiaPagamentoContaConfig.CodigoContaCreditoFk (1 tài khoản Crédito chung) thành
-- 2 tài khoản riêng theo loại khu vực (Setor) của Entidade đóng góp — xác nhận từ sổ
-- sách thật (SCFSSTL2024_VF.xlsm "Lançamentos"): Crédito thực tế dùng 2 tài khoản khác
-- nhau, không phải 1:
--   2133114 "Setor Privado - Guias Emitidas"  (~24.176 dòng, đa số)
--   2133112 "Setor Público - Guias Emitidas"  (~191 dòng)
-- Trước migration này, hệ thống chỉ có 1 tài khoản Crédito cố định — mọi giao dịch của
-- công ty Setor Público bị ghi nhầm vào tài khoản Privado.
--
-- Phân loại Público/Privado lấy từ Entidadeempregadora.EntidadeSectorActFk ->
-- Sectoractividade.Descricao (field có sẵn, bắt buộc nhập khi đăng ký công ty — KHÔNG
-- phải field mới/không có như phiên trước từng kết luận nhầm). Mọi dòng Sectoractividade
-- có Descricao bắt đầu bằng "Setor Público" -> dùng CodigoContaCreditoPublicoFk; còn lại
-- (Setor Privado, PPP, hoặc thiếu dữ liệu) -> dùng CodigoContaCreditoPrivadoFk.
--
-- Bảng GuiaPagamentoContaConfig hiện đang RỖNG (0 dòng, chưa cấu hình giá trị thật) nên
-- an toàn để đổi schema trực tiếp, không cần migrate dữ liệu.
-- Đã backup DB trước khi chạy: db_backup/dated_backups/TimorINSSModuloContribuicoes_2026-07-13_pre_gp_sector_credito_split.bak

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCredito]
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] DROP COLUMN [CodigoContaCreditoFk]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] ADD [CodigoContaCreditoPrivadoFk] [int] NULL
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] ADD [CodigoContaCreditoPublicoFk] [int] NULL
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPrivado]
	FOREIGN KEY([CodigoContaCreditoPrivadoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPrivado]
GO

ALTER TABLE [dbo].[GuiaPagamentoContaConfig] WITH CHECK ADD CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPublico]
	FOREIGN KEY([CodigoContaCreditoPublicoFk]) REFERENCES [dbo].[Codigoconta] ([Id])
GO
ALTER TABLE [dbo].[GuiaPagamentoContaConfig] CHECK CONSTRAINT [FK_GuiaPagamentoContaConfig_CodigoContaCreditoPublico]
GO

-- Cấu hình tích hợp — bật/tắt cho phép các API gọi vào từ module ngoài
-- (hiện chỉ có Benefit module gọi vào api/benefit + api/benefit-data để lấy
-- thông tin NLĐ/công ty/lịch sử đóng góp). Singleton config (Id luôn = 1).
-- Nhớ backup DB trước khi chạy migration này (theo quy ước db_backup/dated_backups).

CREATE TABLE [dbo].[IntegrationConfig](
	[Id] [int] NOT NULL,
	[BenefitApiEnabled] [bit] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	CONSTRAINT [PK_IntegrationConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Default row: bật sẵn (đang có traffic thật từ Benefit module dùng API này
-- rồi), admin có thể tắt bất cứ lúc nào từ màn "Cấu hình tích hợp".
INSERT INTO [dbo].[IntegrationConfig]
	([Id], [BenefitApiEnabled])
VALUES
	(1, 1)
GO

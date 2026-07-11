-- Granular RBAC for the new-mode ("Quản lý User & Phân quyền" screen).
-- Additive only — does NOT touch the old Perfil/Funcionalidade/RelUtilizadorPerfil
-- system (that keeps serving old-mode accounts unchanged) and does NOT touch
-- UserModeAccess (that stays a separate binary new-mode gate).
--
-- The permission-TOKEN catalog itself (ORC_SUBMIT, AD_APPROVE, ...) is a fixed
-- list in backend code (Common/PermissionCatalog.cs), not a DB table — only
-- grants (UserPermission) and convenience bundles (PermissionPreset) are data.
--
-- NOTE: no [RequirePerm]-style enforcement is wired into any controller yet
-- (deliberately out of scope this round) — this migration only lays down the
-- storage so the admin screen can assign tokens; enforcement is a follow-up.

CREATE TABLE [dbo].[PermissionPreset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](50) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_PermissionPreset] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_PermissionPreset_Codigo] ON [dbo].[PermissionPreset] ([Codigo])
	WHERE [IndActivo] = 1
GO

CREATE TABLE [dbo].[PermissionPresetItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PermissionPresetFk] [int] NOT NULL,
	[PermissionToken] [varchar](50) NOT NULL,
	CONSTRAINT [PK_PermissionPresetItem] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PermissionPresetItem] WITH CHECK ADD CONSTRAINT [FK_PermissionPresetItem_Preset]
	FOREIGN KEY([PermissionPresetFk]) REFERENCES [dbo].[PermissionPreset] ([Id])
GO
ALTER TABLE [dbo].[PermissionPresetItem] CHECK CONSTRAINT [FK_PermissionPresetItem_Preset]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_PermissionPresetItem_Preset_Token] ON [dbo].[PermissionPresetItem] ([PermissionPresetFk], [PermissionToken])
GO

CREATE TABLE [dbo].[UserPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[PermissionToken] [varchar](50) NOT NULL,
	-- NULL = granted individually; set = expanded from applying a preset (so
	-- re-applying/removing a preset can be told apart from a manual grant)
	[SourcePresetFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserPermission] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserPermission] WITH CHECK ADD CONSTRAINT [FK_UserPermission_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[Utilizador] ([IdUtilizador])
GO
ALTER TABLE [dbo].[UserPermission] CHECK CONSTRAINT [FK_UserPermission_Utilizador]
GO
ALTER TABLE [dbo].[UserPermission] WITH CHECK ADD CONSTRAINT [FK_UserPermission_Preset]
	FOREIGN KEY([SourcePresetFk]) REFERENCES [dbo].[PermissionPreset] ([Id])
GO
ALTER TABLE [dbo].[UserPermission] CHECK CONSTRAINT [FK_UserPermission_Preset]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_UserPermission_User_Token] ON [dbo].[UserPermission] ([UtilizadorFk], [PermissionToken])
	WHERE [IndActivo] = 1
GO

-- Starter bundles — a sensible default mapped from the legal approval chain
-- (ReuniaoSS_2024.pptx: Técnico -> Diretor Departamento Financeiro -> Diretor
-- Executivo), editable later; NOT a locked design, just a convenient seed.
INSERT INTO [dbo].[PermissionPreset] (Codigo, Nome, IndActivo, UtilizadorCriacao, DataCriacao) VALUES
	('ADMIN', N'Quản trị hệ thống (toàn quyền)', 1, 2, GETDATE()),
	('TECNICO_DF', N'Técnico Departamento Financeiro (nhập liệu / submit)', 1, 2, GETDATE()),
	('DIRETOR_DF', N'Diretor Departamento Financeiro (kiểm tra / duyệt cấp 1)', 1, 2, GETDATE()),
	('DIRETOR_EXECUTIVO', N'Diretor Executivo (phê duyệt cấp cao)', 1, 2, GETDATE()),
	('REPORT_ONLY', N'Chỉ xem báo cáo', 1, 2, GETDATE())
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT Id, 'ADMIN' FROM [dbo].[PermissionPreset] WHERE Codigo = 'ADMIN'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_SUBMIT'), ('AD_SUBMIT'), ('CABIMENTO_SUBMIT'), ('COMPROMISSO_SUBMIT'),
	('OBRIGACAO_SUBMIT'), ('PAG_SUBMIT'), ('REC_SUBMIT'), ('REPORT_VIEW')
) AS t(Token)
WHERE p.Codigo = 'TECNICO_DF'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_REVIEW'), ('AD_REVIEW'), ('CABIMENTO_APPROVE'), ('COMPROMISSO_REVIEW'),
	('OBRIGACAO_APPROVE'), ('PAG_APPROVE'), ('REC_REVIEW'), ('BANCO_CONCILIAR'),
	('REPORT_VIEW'), ('MASTERDATA_MANAGE')
) AS t(Token)
WHERE p.Codigo = 'DIRETOR_DF'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT p.Id, t.Token
FROM [dbo].[PermissionPreset] p
CROSS APPLY (VALUES
	('ORC_APPROVE'), ('AD_APPROVE'), ('COMPROMISSO_APPROVE'), ('REC_APPROVE'),
	('ABE_APPROVE'), ('PAG_EXECUTE'), ('REPORT_VIEW')
) AS t(Token)
WHERE p.Codigo = 'DIRETOR_EXECUTIVO'
GO

INSERT INTO [dbo].[PermissionPresetItem] (PermissionPresetFk, PermissionToken)
SELECT Id, 'REPORT_VIEW' FROM [dbo].[PermissionPreset] WHERE Codigo = 'REPORT_ONLY'
GO

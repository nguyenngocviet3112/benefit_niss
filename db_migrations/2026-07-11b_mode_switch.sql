-- Mode switch — gates which users can enter the new Contabilidade mode.
-- Interim mechanism: presence of an ACTIVE row = has access. This is a
-- placeholder gate; it gets superseded by the full granular UserPermission
-- system when "Quản lý User & Phân quyền" is built — do not extend this
-- table with more columns, replace it instead.

CREATE TABLE [dbo].[UserModeAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserModeAccess] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserModeAccess] WITH CHECK ADD CONSTRAINT [FK_UserModeAccess_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[UTILIZADOR] ([idUtilizador])
GO
ALTER TABLE [dbo].[UserModeAccess] CHECK CONSTRAINT [FK_UserModeAccess_Utilizador]
GO

CREATE UNIQUE INDEX [UX_UserModeAccess_Utilizador] ON [dbo].[UserModeAccess] ([UtilizadorFk])
GO

-- Seed: grant the 'admin' user (idUtilizador=1) access for testing the switch.
INSERT INTO [dbo].[UserModeAccess] ([UtilizadorFk], [IndActivo], [UtilizadorCriacao], [DataCriacao])
VALUES (1, 1, 1, GETDATE())
GO

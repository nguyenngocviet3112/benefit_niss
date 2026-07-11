-- Extra profile fields for the "Quản lý User & Phân quyền" screen (user asked
-- for Tên/Email/Phòng ban in addition to Username/Password, 2026-07-11) —
-- Email is meant to back a future password-reset flow (not built yet, just
-- the data capture for now).
--
-- New satellite table instead of ALTER TABLE Utilizador — per house rule
-- "prioritize creating new tables over modifying fields of an existing
-- table" (2026-07-11). DepartamentoFk reuses the existing Departamento
-- master-data table (already used elsewhere, e.g. Relutilizadordepartamento)
-- for the dropdown — does not touch that old table/relationship.

CREATE TABLE [dbo].[UserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UtilizadorFk] [int] NOT NULL,
	[Nome] [nvarchar](200) NULL,
	[Email] [nvarchar](200) NULL,
	[DepartamentoFk] [int] NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserProfile] WITH CHECK ADD CONSTRAINT [FK_UserProfile_Utilizador]
	FOREIGN KEY([UtilizadorFk]) REFERENCES [dbo].[Utilizador] ([IdUtilizador])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Utilizador]
GO
ALTER TABLE [dbo].[UserProfile] WITH CHECK ADD CONSTRAINT [FK_UserProfile_Departamento]
	FOREIGN KEY([DepartamentoFk]) REFERENCES [dbo].[Departamento] ([Id])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Departamento]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE UNIQUE INDEX [UX_UserProfile_Utilizador] ON [dbo].[UserProfile] ([UtilizadorFk])
	WHERE [IndActivo] = 1
GO

-- File attachment support for the expenditure cycle (AD/Cabimento/Compromisso/
-- Obrigação/Pagamento) — user requested 2026-07-13: submitter can attach a file
-- when submitting (PDF/PNG/Excel), view it back later, admin can configure the
-- max upload size and which stages require a mandatory attachment before submit
-- is allowed.
--
-- Attachment is a generic/polymorphic table (EntityType + EntityId) rather than
-- 5 separate FK'd tables — same shape across all 5 stages, and this list of
-- entity types will keep growing (see house convention: additive satellite
-- tables). File bytes stored as varbinary(max), same pattern as the existing
-- old-mode Componentedocumentoregisto/Documentoidentificacao tables (no disk/
-- blob storage exists anywhere in this codebase yet — see attachment-feature
-- research, 2026-07-13).

CREATE TABLE [dbo].[Attachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	-- 'AD' | 'CABIMENTO' | 'COMPROMISSO' | 'OBRIGACAO' | 'PAGAMENTO'
	[EntityType] [varchar](30) NOT NULL,
	[EntityId] [int] NOT NULL,
	[FileName] [nvarchar](260) NOT NULL,
	[ContentType] [varchar](150) NOT NULL,
	[FileSize] [int] NOT NULL,
	[FileContent] [varbinary](max) NOT NULL,
	[IndActivo] [bit] NOT NULL,
	[UtilizadorCriacao] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	[Ipv6] [varchar](45) NULL,
	CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE INDEX [IX_Attachment_Entity] ON [dbo].[Attachment] ([EntityType], [EntityId])
	WHERE [IndActivo] = 1
GO

-- Singleton config row (Id always = 1) — max upload size (shared across all 5
-- stages, one system-wide setting per user's request) + per-stage "must attach
-- before submit" flags.
CREATE TABLE [dbo].[AttachmentConfig](
	[Id] [int] NOT NULL,
	[MaxFileSizeMb] [int] NOT NULL,
	[AdObrigatorio] [bit] NOT NULL,
	[CabimentoObrigatorio] [bit] NOT NULL,
	[CompromissoObrigatorio] [bit] NOT NULL,
	[ObrigacaoObrigatorio] [bit] NOT NULL,
	[PagamentoObrigatorio] [bit] NOT NULL,
	[UtilizadorAlteracao] [int] NULL,
	[DataAlteracao] [datetime] NULL,
	CONSTRAINT [PK_AttachmentConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Default row: 10MB limit, nothing mandatory yet (admin opts in per stage from
-- the new settings screen) — safe default that doesn't block any in-flight
-- DRAFT submissions the moment this migration runs.
INSERT INTO [dbo].[AttachmentConfig]
	([Id], [MaxFileSizeMb], [AdObrigatorio], [CabimentoObrigatorio], [CompromissoObrigatorio], [ObrigacaoObrigatorio], [PagamentoObrigatorio])
VALUES
	(1, 10, 0, 0, 0, 0, 0)
GO

-- Bổ sung nhánh con còn thiếu của A07 (Administração do FRSS) trong
-- ProgramActivity — phát hiện qua audit đối chiếu với OSS_Global_2026_FINAL_livro.xlsx
-- (sheet Sintese_ProgramasOSS_CE), xem
-- supported_documents/sample_import_data/MASTER_DATA_COMPLETENESS_CHECK.md.
-- A07 trước đó chỉ có 1 dòng gốc (thêm ở 2026-07-11f_oss_perimeter.sql),
-- thiếu hẳn cấu trúc Subprograma/Atividade như A04/A05/A06 đã có.
-- IsOssPerimeter = 1 vì A07 nằm TRONG perimeter OSS (kế thừa từ dòng cha).
-- Run against a DEV/local copy first. Back up with a dated label before
-- running against any shared environment.

DECLARE @a07 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A07');

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A0701', N'Coordenação, gestão e funcionamento do FRSS', 2, @a07, 1, 1, 1, 1, GETDATE()),
	('A0702', N'Investimento Estratégico do FRSS', 2, @a07, 1, 1, 1, 1, GETDATE())
GO

DECLARE @a0701 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A0701');
DECLARE @a0702 INT = (SELECT Id FROM ProgramActivity WHERE Codigo = 'A0702');

INSERT INTO [dbo].[ProgramActivity]
	([Codigo], [Designacao], [Nivel], [ParentFk], [OrcamentoConfigFk], [IndActivo], [IsOssPerimeter], [UtilizadorCriacao], [DataCriacao])
VALUES
	('A070101', N'Funcionamento de todos os serviços e unidades orgânicas do Instituto FRSS', 3, @a0701, 1, 1, 1, 1, GETDATE()),
	('A070102', N'Capacitação técnica e especializada dos Recursos Humanos do FRSS', 3, @a0701, 1, 1, 1, 1, GETDATE()),
	('A070201', N'Instalação e Aquisição de Equipamentos no FRSS', 3, @a0702, 1, 1, 1, 1, GETDATE())
GO

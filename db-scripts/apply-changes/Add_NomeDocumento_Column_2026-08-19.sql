-- ============================================================================
-- Add COMPONENTEDOCUMENTO_REGISTO.nomeDocumento                      2026-08-19
-- ============================================================================
-- [EN] Why: when a document is attached to a task, the browser already sends the
-- file name, but the backend discarded it -- only the document TYPE was stored.
-- The "Documents" table could therefore only show the type, so two different
-- files of the same type looked identical, and there was no way to tell which
-- file had been uploaded.
--
-- This adds one nullable column to hold the file name. From then on new uploads
-- record it; documents attached before this change keep it empty, which the
-- screen shows as a dash.
--
-- Required by the application build of 2026-08-19 -- the code reads and writes
-- this column, so run this BEFORE deploying that build.
--
-- Safe to run repeatedly: it checks first and does nothing if the column exists.
-- Adds a nullable column, so no existing row is rewritten and no data is lost.
--
-- BACK UP THE DATABASE BEFORE RUNNING (standard rule).
--
-- [VI] Khi đính kèm tài liệu vào tarefa, trình duyệt vốn đã gửi tên file nhưng
-- backend bỏ đi -- chỉ lưu LOẠI tài liệu. Vì vậy bảng "Documents" chỉ hiện được
-- loại, hai file khác nhau cùng loại trông y hệt nhau và không cách nào biết đã
-- tải file nào lên. Script thêm 1 cột nullable để lưu tên file. Tài liệu cũ để
-- trống (màn hình hiện dấu gạch ngang). Cần chạy TRƯỚC khi deploy bản 19/08/2026.
-- Chạy lại nhiều lần vô hại; thêm cột nullable nên không ghi đè dòng nào.
-- ============================================================================

SET NOCOUNT ON;

IF OBJECT_ID('dbo.COMPONENTEDOCUMENTO_REGISTO') IS NULL
BEGIN
    PRINT '*** STOPPED: table COMPONENTEDOCUMENTO_REGISTO does not exist on this database.';
    RETURN;
END

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('dbo.COMPONENTEDOCUMENTO_REGISTO') AND name = 'nomeDocumento')
BEGIN
    PRINT 'Column nomeDocumento already exists -- nothing to do.';
END
ELSE
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.COMPONENTEDOCUMENTO_REGISTO ADD nomeDocumento VARCHAR(255) NULL;
        PRINT 'CREATED: COMPONENTEDOCUMENTO_REGISTO.nomeDocumento varchar(255) NULL.';
    END TRY
    BEGIN CATCH
        PRINT CONCAT('*** FAILED: ', ERROR_MESSAGE());
        RETURN;
    END CATCH
END

PRINT '';
PRINT 'Verification:';

SELECT c.name AS coluna, t.name AS tipo, c.max_length AS tamanho, c.is_nullable AS aceitaNulo
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.COMPONENTEDOCUMENTO_REGISTO') AND c.name = 'nomeDocumento';

-- [EN] Dynamic SQL on purpose: SQL Server compiles the whole batch before running it,
-- so referencing the new column directly would fail to parse on a database where the
-- column does not exist yet -- taking the ALTER above down with it.
-- [VI] Cố tình dùng dynamic SQL: SQL Server biên dịch cả batch trước khi chạy, nên nếu
-- tham chiếu thẳng cột mới thì trên DB chưa có cột sẽ lỗi ngay lúc phân tích cú pháp --
-- kéo theo cả lệnh ALTER phía trên không chạy được.
EXEC('SELECT COUNT(*) AS documentosSemNome
      FROM COMPONENTEDOCUMENTO_REGISTO
      WHERE indActivo = 1 AND (nomeDocumento IS NULL OR nomeDocumento = '''')');

PRINT '';
PRINT 'DONE. Documents attached before this change have no file name -- that is expected.';

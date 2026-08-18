-- ============================================================================
-- DB Health Check + repair -- 2026-08-18
-- ============================================================================
-- [EN] Purpose: one single script that checks whether this database has every
-- schema change the current application code expects, reports each item as
-- PRESENT or MISSING, and creates whatever is missing.
--
-- Why it exists: a database restored from an older backup silently loses the
-- columns/tables added by later upgrades. The application then fails at the
-- exact screen that uses them -- one error at a time, weeks apart. Running this
-- script once after any restore surfaces all of them at once.
--
-- Safe to run repeatedly: every step checks first and skips what already exists.
-- It never drops or deletes anything, and it never changes any business data.
-- Only structural changes (add column / widen column) are performed.
--
-- BACK UP THE DATABASE BEFORE RUNNING (standard rule for any migration).
--
-- [VI] Mục đích: 1 script duy nhất kiểm tra database có đủ mọi thay đổi schema
-- mà code hiện tại cần hay không, in PRESENT/MISSING từng mục, và tạo phần thiếu.
-- Lý do: DB restore từ backup cũ sẽ mất âm thầm các cột/bảng do bản nâng cấp sau
-- thêm vào; app chỉ lỗi khi vào đúng màn dùng tới -- mỗi lần 1 lỗi, cách nhau
-- hàng tuần. Chạy script này 1 lần sau mỗi lần restore là lộ hết cùng lúc.
-- An toàn khi chạy lại nhiều lần; không xoá gì, không đụng dữ liệu nghiệp vụ.
-- ============================================================================

SET NOCOUNT ON;

PRINT '====================================================================';
PRINT ' DB HEALTH CHECK -- starting';
PRINT '====================================================================';
PRINT '';

-- ============================================================================
-- PART 1 -- Schema items this script can create/repair by itself
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1.1  PAGAMENTOSEXECUTADOS.bankCode
--      Needed by: "Executar Pagamentos" pop-up (Bank dropdown next to Numero
--      Conta). Added by the application upgrade of 16/07/2026. Any string
--      column of length 50 or more is accepted.
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.PAGAMENTOSEXECUTADOS') IS NULL
    PRINT '1.1 bankCode           : SKIPPED -- table PAGAMENTOSEXECUTADOS does not exist on this database.';
ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.PAGAMENTOSEXECUTADOS') AND name = 'bankCode')
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.PAGAMENTOSEXECUTADOS ADD bankCode VARCHAR(50) NULL;
        PRINT '1.1 bankCode           : CREATED -- column PAGAMENTOSEXECUTADOS.bankCode varchar(50) NULL added.';
    END TRY
    BEGIN CATCH
        PRINT CONCAT('1.1 bankCode           : *** FAILED -- ', ERROR_MESSAGE());
    END CATCH
END
ELSE
    PRINT '1.1 bankCode           : already present, nothing to do.';

-- ----------------------------------------------------------------------------
-- 1.2  COMPONENTERECEITA_REGISTO.InstitutionId
--      Needed by: "Registar Receita" (Institution dropdown) and by the monthly
--      Receita columns of the CE_OSS_Global report, which filter on it.
--      NOTE: adding the column leaves existing Receita rows with NULL. Those
--      rows stay invisible in the report until the separate backfill script
--      db-migrations/backfill_receita_institutionid_2026-07-28.sql is run --
--      part 2.4 below reports how many such rows exist.
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') IS NULL
    PRINT '1.2 InstitutionId      : SKIPPED -- table COMPONENTERECEITA_REGISTO does not exist on this database.';
ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') AND name = 'InstitutionId')
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.COMPONENTERECEITA_REGISTO ADD InstitutionId INT NULL;
        PRINT '1.2 InstitutionId      : CREATED -- column COMPONENTERECEITA_REGISTO.InstitutionId int NULL added.';
    END TRY
    BEGIN CATCH
        PRINT CONCAT('1.2 InstitutionId      : *** FAILED -- ', ERROR_MESSAGE());
    END CATCH
END
ELSE
    PRINT '1.2 InstitutionId      : already present, nothing to do.';

-- Foreign key to INSTITUTION -- only if both sides exist and no FK is defined yet.
IF OBJECT_ID('dbo.INSTITUTION') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') AND name = 'InstitutionId')
   AND NOT EXISTS (
        SELECT 1 FROM sys.foreign_key_columns fkc
        JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
        WHERE fkc.parent_object_id = OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') AND c.name = 'InstitutionId')
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.COMPONENTERECEITA_REGISTO
            ADD CONSTRAINT FK_COMPONENTERECEITA_REGISTO_INSTITUTION
            FOREIGN KEY (InstitutionId) REFERENCES dbo.INSTITUTION (id);
        PRINT '    -> foreign key to INSTITUTION created.';
    END TRY
    BEGIN CATCH
        PRINT CONCAT('    -> foreign key NOT created (not fatal, the application does not require it): ', ERROR_MESSAGE());
    END CATCH
END

-- ----------------------------------------------------------------------------
-- 1.3  COMPONENTEDESPESA_REGISTO.descricao -- must hold at least 150 characters
--      Needed by: Registo de Despesa. Shipped 16/07/2026 as part of
--      DB_Change_2026-07-16_ipv6_and_despesa_descricao.sql. If the column is
--      still varchar(100), saving a longer description fails with
--      "String or binary data would be truncated".
-- ----------------------------------------------------------------------------
DECLARE @descLen INT = (
    SELECT CASE WHEN t.name IN ('nvarchar','nchar') THEN c.max_length / 2 ELSE c.max_length END
    FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.COMPONENTEDESPESA_REGISTO') AND c.name = 'descricao');

IF @descLen IS NULL
    PRINT '1.3 despesa descricao  : SKIPPED -- column COMPONENTEDESPESA_REGISTO.descricao not found.';
ELSE IF @descLen <> -1 AND @descLen < 150
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.COMPONENTEDESPESA_REGISTO ALTER COLUMN descricao VARCHAR(150) NOT NULL;
        PRINT CONCAT('1.3 despesa descricao  : WIDENED -- was varchar(', @descLen, '), now varchar(150).');
    END TRY
    BEGIN CATCH
        PRINT CONCAT('1.3 despesa descricao  : *** FAILED -- ', ERROR_MESSAGE());
    END CATCH
END
ELSE
    PRINT CONCAT('1.3 despesa descricao  : already wide enough (', CASE WHEN @descLen = -1 THEN 'max' ELSE CAST(@descLen AS VARCHAR(10)) END, ').');

-- ----------------------------------------------------------------------------
-- 1.4  ipv6 columns -- must hold at least 45 characters
--      Needed everywhere: the application writes the caller IP into ipv6 on
--      almost every table. IPv6 addresses do not fit in the original
--      varchar(16), which makes saving fail with "String or binary data would
--      be truncated" on whichever screen is used from an IPv6 client.
--      This step scans EVERY table rather than a fixed list, so it stays
--      correct even if new tables are added later.
-- ----------------------------------------------------------------------------
DECLARE @ipv6Total INT, @ipv6Narrow INT, @ipv6Fixed INT = 0, @ipv6Failed INT = 0;

SELECT @ipv6Total = COUNT(*) FROM sys.columns c JOIN sys.tables tb ON c.object_id = tb.object_id WHERE c.name = 'ipv6';

DECLARE @tbl SYSNAME, @sch SYSNAME, @typ SYSNAME, @nullable BIT, @sql NVARCHAR(500);

DECLARE ipv6cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT s.name, tb.name, t.name, c.is_nullable
    FROM sys.columns c
    JOIN sys.tables tb ON c.object_id = tb.object_id
    JOIN sys.schemas s ON tb.schema_id = s.schema_id
    JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.name = 'ipv6'
      AND t.name IN ('varchar','nvarchar','char','nchar')
      AND c.max_length <> -1
      AND (CASE WHEN t.name IN ('nvarchar','nchar') THEN c.max_length / 2 ELSE c.max_length END) < 45;

OPEN ipv6cur;
FETCH NEXT FROM ipv6cur INTO @sch, @tbl, @typ, @nullable;
SET @ipv6Narrow = 0;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ipv6Narrow = @ipv6Narrow + 1;
    SET @sql = CONCAT('ALTER TABLE [', @sch, '].[', @tbl, '] ALTER COLUMN [ipv6] ',
                      CASE WHEN @typ IN ('nvarchar','nchar') THEN 'nvarchar' ELSE 'varchar' END,
                      '(45) ', CASE WHEN @nullable = 1 THEN 'NULL' ELSE 'NOT NULL' END, ';');
    BEGIN TRY
        EXEC sp_executesql @sql;
        SET @ipv6Fixed = @ipv6Fixed + 1;
    END TRY
    BEGIN CATCH
        SET @ipv6Failed = @ipv6Failed + 1;
        PRINT CONCAT('    -> could not widen ipv6 on [', @sch, '].[', @tbl, ']: ', ERROR_MESSAGE());
    END CATCH
    FETCH NEXT FROM ipv6cur INTO @sch, @tbl, @typ, @nullable;
END

CLOSE ipv6cur;
DEALLOCATE ipv6cur;

IF @ipv6Narrow = 0
    PRINT CONCAT('1.4 ipv6 columns       : all ', @ipv6Total, ' ipv6 column(s) are already 45 characters or wider.');
ELSE
    PRINT CONCAT('1.4 ipv6 columns       : ', @ipv6Fixed, ' of ', @ipv6Narrow, ' narrow column(s) widened to 45 (out of ', @ipv6Total, ' ipv6 columns in total)',
                 CASE WHEN @ipv6Failed > 0 THEN CONCAT(' -- ', @ipv6Failed, ' FAILED, see the messages above.') ELSE '.' END);

PRINT '';

-- ============================================================================
-- PART 2 -- Items this script only REPORTS (they belong to their own scripts)
-- ============================================================================

DECLARE @ceTable BIT = CASE WHEN OBJECT_ID('dbo.RELAGRUPAMENTOCONFIGCLASSIFICACAOECONOMICA') IS NOT NULL THEN 1 ELSE 0 END;

IF @ceTable = 1
    PRINT '2.1 CE crosswalk table : PRESENT.';
ELSE
    PRINT '2.1 CE crosswalk table : *** MISSING -- the OSS Global report will fail with "Invalid object name". Run db-migrations/DB_Change_CE_OSS_Global_2026-07-18.sql (revision 2026-08-18).';

-- A07 Programa branch (created by the CE_OSS_Global script, not by this one)
DECLARE @a07 INT, @a0701 INT, @a070101 INT, @a07ok BIT = 0;

SELECT @a07 = a.id
FROM AGRUPAMENTOCONFIG a
JOIN RELTIPODECONTAORCAMENTOCONFIG r ON a.reltipoDeContaOrcamentoConfig_fk = r.id
JOIN DOMINIO d ON r.tipoConta_fk = d.idDominio
WHERE d.dominio = 'TIPOCONTA' AND d.descricao = 'Actidade'
  AND a.codigo = 'A07' AND a.parent_fk IS NULL AND a.indActivo = 1;

SELECT @a0701   = id FROM AGRUPAMENTOCONFIG WHERE parent_fk = @a07     AND codigo = '01' AND indActivo = 1;
SELECT @a070101 = id FROM AGRUPAMENTOCONFIG WHERE parent_fk = @a0701   AND codigo = '01' AND indActivo = 1;

IF @a07 IS NOT NULL AND @a0701 IS NOT NULL AND @a070101 IS NOT NULL SET @a07ok = 1;

IF @a07ok = 1
    PRINT '2.2 A07 Programa branch: PRESENT (all 3 rows).';
ELSE
    PRINT '2.2 A07 Programa branch: *** MISSING -- the A07 / FRSS branch will not appear in Programa dropdowns or in reports broken down by Programa. Run db-migrations/DB_Change_CE_OSS_Global_2026-07-18.sql (revision 2026-08-18).';

-- TIPOCONTA domain values required by the budget/expenditure screens
DECLARE @domAct BIT = CASE WHEN EXISTS (SELECT 1 FROM DOMINIO WHERE dominio = 'TIPOCONTA' AND descricao = 'Actidade')  THEN 1 ELSE 0 END;
DECLARE @domFun BIT = CASE WHEN EXISTS (SELECT 1 FROM DOMINIO WHERE dominio = 'TIPOCONTA' AND descricao = 'Funcional') THEN 1 ELSE 0 END;

IF @domAct = 1 AND @domFun = 1
    PRINT '2.3 TIPOCONTA values   : PRESENT (Actidade + Funcional).';
ELSE
    PRINT CONCAT('2.3 TIPOCONTA values   : *** MISSING -- ',
                 CASE WHEN @domAct = 0 THEN 'Actidade ' ELSE '' END,
                 CASE WHEN @domFun = 0 THEN 'Funcional ' ELSE '' END,
                 '-- run db-migrations/DB_Change_CE_OSS_Global_2026-07-18.sql (revision 2026-08-18), which creates them.');

-- Receita rows still without an Institution
DECLARE @receitaNull INT = NULL;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') AND name = 'InstitutionId')
    SELECT @receitaNull = COUNT(*) FROM COMPONENTERECEITA_REGISTO WHERE InstitutionId IS NULL;

IF @receitaNull IS NULL
    PRINT '2.4 Receita Institution: not checked (column missing -- see 1.2 above).';
ELSE IF @receitaNull = 0
    PRINT '2.4 Receita Institution: OK -- every Receita row has an Institution.';
ELSE
    PRINT CONCAT('2.4 Receita Institution: ', @receitaNull, ' Receita row(s) have no Institution and are therefore EXCLUDED from the Receita columns of the OSS Global report. Fix by running db-migrations/backfill_receita_institutionid_2026-07-28.sql.');

PRINT '';

-- ============================================================================
-- PART 3 -- Summary
-- ============================================================================
DECLARE @okBank BIT = CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.PAGAMENTOSEXECUTADOS') AND name = 'bankCode') THEN 1 ELSE 0 END;
DECLARE @okInst BIT = CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.COMPONENTERECEITA_REGISTO') AND name = 'InstitutionId') THEN 1 ELSE 0 END;
DECLARE @okDesc BIT = CASE WHEN (SELECT CASE WHEN t.name IN ('nvarchar','nchar') THEN c.max_length / 2 ELSE c.max_length END
                                 FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
                                 WHERE c.object_id = OBJECT_ID('dbo.COMPONENTEDESPESA_REGISTO') AND c.name = 'descricao') NOT IN (-1) THEN
                            CASE WHEN (SELECT CASE WHEN t.name IN ('nvarchar','nchar') THEN c.max_length / 2 ELSE c.max_length END
                                       FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
                                       WHERE c.object_id = OBJECT_ID('dbo.COMPONENTEDESPESA_REGISTO') AND c.name = 'descricao') >= 150 THEN 1 ELSE 0 END
                       ELSE 1 END;
DECLARE @okIpv6 BIT = CASE WHEN NOT EXISTS (
        SELECT 1 FROM sys.columns c JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.name = 'ipv6' AND c.max_length <> -1
          AND (CASE WHEN t.name IN ('nvarchar','nchar') THEN c.max_length / 2 ELSE c.max_length END) < 45) THEN 1 ELSE 0 END;

PRINT '====================================================================';
PRINT ' SUMMARY';
PRINT '--------------------------------------------------------------------';
PRINT CONCAT('  PAGAMENTOSEXECUTADOS.bankCode              : ', CASE WHEN @okBank    = 1 THEN 'PRESENT' ELSE 'MISSING' END);
PRINT CONCAT('  COMPONENTERECEITA_REGISTO.InstitutionId    : ', CASE WHEN @okInst    = 1 THEN 'PRESENT' ELSE 'MISSING' END);
PRINT CONCAT('  COMPONENTEDESPESA_REGISTO.descricao >= 150 : ', CASE WHEN @okDesc    = 1 THEN 'PRESENT' ELSE 'TOO SHORT' END);
PRINT CONCAT('  ipv6 columns >= 45 characters              : ', CASE WHEN @okIpv6    = 1 THEN 'PRESENT' ELSE 'TOO SHORT' END);
PRINT CONCAT('  CE crosswalk table                         : ', CASE WHEN @ceTable   = 1 THEN 'PRESENT' ELSE 'MISSING -- run the CE_OSS_Global script' END);
PRINT CONCAT('  A07 Programa branch (3 rows)               : ', CASE WHEN @a07ok     = 1 THEN 'PRESENT' ELSE 'MISSING -- run the CE_OSS_Global script' END);
PRINT CONCAT('  TIPOCONTA Actidade + Funcional             : ', CASE WHEN @domAct = 1 AND @domFun = 1 THEN 'PRESENT' ELSE 'MISSING -- run the CE_OSS_Global script' END);
PRINT '--------------------------------------------------------------------';

IF @okBank = 1 AND @okInst = 1 AND @okDesc = 1 AND @okIpv6 = 1 AND @ceTable = 1 AND @a07ok = 1 AND @domAct = 1 AND @domFun = 1
    PRINT ' RESULT: this database has everything the current application needs.';
ELSE
    PRINT ' RESULT: *** ITEMS ARE STILL MISSING -- see the lines marked MISSING above. Anything reported as "run the CE_OSS_Global script" is fixed by that script, not by this one. Everything else can be fixed by simply running this script again.';

PRINT '====================================================================';

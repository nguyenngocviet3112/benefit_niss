-- ============================================================================
-- Backfill DESTINATARIO.nome / DESTINATARIO.NISS                    2026-08-22
-- ============================================================================
-- WHY
-- The payment order PDF ("Lista Pagamentu Salariu Funcionariu INSS") printed an
-- empty NISS and an empty recipient name on every line, while the account number
-- and IBAN were correct.
--
-- Cause: those DESTINATARIO rows carry no name and no NISS of their own. They do
-- carry a valid trabalhador_fk (or entidade_fk), and the linked TRABALHADOR /
-- ENTIDADEEMPREGADORA record holds the correct name and NISS. Nothing was lost --
-- the values were simply never copied onto the DESTINATARIO row.
--
-- This script copies them across. It only ever fills a field that is currently
-- empty; a recipient who already has a name keeps it untouched.
--
-- Safe to run repeatedly: rows fixed by an earlier run are no longer empty, so a
-- second run finds nothing to do and reports 0 updated.
--
-- BACK UP THE DATABASE BEFORE RUNNING (standard rule for this folder).
--
-- Run on EVERY environment -- Dev, Staging and Production. Data fixes do not
-- travel with a code deployment.
-- ============================================================================

SET NOCOUNT ON;

IF OBJECT_ID('dbo.DESTINATARIO') IS NULL
BEGIN
    PRINT '*** STOPPED: table DESTINATARIO does not exist on this database.';
    RETURN;
END

PRINT 'Before:';

SELECT
    COUNT(*)                                                                    AS totalRecipients,
    SUM(CASE WHEN nome IS NULL OR nome = '' THEN 1 ELSE 0 END)                  AS missingName,
    SUM(CASE WHEN NISS IS NULL OR NISS = '' THEN 1 ELSE 0 END)                  AS missingNiss,
    SUM(CASE WHEN (nome IS NULL OR nome = '') AND trabalhador_fk IS NOT NULL
             THEN 1 ELSE 0 END)                                                 AS fixableFromWorker,
    SUM(CASE WHEN (nome IS NULL OR nome = '') AND entidade_fk    IS NOT NULL
             THEN 1 ELSE 0 END)                                                 AS fixableFromEmployer,
    SUM(CASE WHEN (nome IS NULL OR nome = '')
              AND trabalhador_fk IS NULL AND entidade_fk IS NULL
             THEN 1 ELSE 0 END)                                                 AS notFixable
FROM dbo.DESTINATARIO
WHERE indActivo = 1;

DECLARE @nameFromWorker    INT = 0, @nissFromWorker    INT = 0;
DECLARE @nameFromEmployer  INT = 0, @nissFromEmployer  INT = 0;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Recipients that are workers -------------------------------------------
    UPDATE d
       SET d.nome = t.nome
      FROM dbo.DESTINATARIO d
      JOIN dbo.TRABALHADOR  t ON t.idTrabalhador = d.trabalhador_fk
     WHERE d.indActivo = 1
       AND (d.nome IS NULL OR d.nome = '')
       AND t.nome IS NOT NULL AND t.nome <> '';
    SET @nameFromWorker = @@ROWCOUNT;

    UPDATE d
       SET d.NISS = t.NISS
      FROM dbo.DESTINATARIO d
      JOIN dbo.TRABALHADOR  t ON t.idTrabalhador = d.trabalhador_fk
     WHERE d.indActivo = 1
       AND (d.NISS IS NULL OR d.NISS = '')
       AND t.NISS IS NOT NULL AND t.NISS <> '';
    SET @nissFromWorker = @@ROWCOUNT;

    -- Recipients that are employers ------------------------------------------
    UPDATE d
       SET d.nome = e.nome
      FROM dbo.DESTINATARIO       d
      JOIN dbo.ENTIDADEEMPREGADORA e ON e.idEntidadeEmpreg = d.entidade_fk
     WHERE d.indActivo = 1
       AND (d.nome IS NULL OR d.nome = '')
       AND e.nome IS NOT NULL AND e.nome <> '';
    SET @nameFromEmployer = @@ROWCOUNT;

    UPDATE d
       SET d.NISS = e.NISS
      FROM dbo.DESTINATARIO       d
      JOIN dbo.ENTIDADEEMPREGADORA e ON e.idEntidadeEmpreg = d.entidade_fk
     WHERE d.indActivo = 1
       AND (d.NISS IS NULL OR d.NISS = '')
       AND e.NISS IS NOT NULL AND e.NISS <> '';
    SET @nissFromEmployer = @@ROWCOUNT;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT CONCAT('*** FAILED, nothing was changed: ', ERROR_MESSAGE());
    RETURN;
END CATCH

PRINT '';
PRINT CONCAT('Names  filled from TRABALHADOR         : ', @nameFromWorker);
PRINT CONCAT('NISS   filled from TRABALHADOR         : ', @nissFromWorker);
PRINT CONCAT('Names  filled from ENTIDADEEMPREGADORA : ', @nameFromEmployer);
PRINT CONCAT('NISS   filled from ENTIDADEEMPREGADORA : ', @nissFromEmployer);
PRINT '';
PRINT 'After:';

SELECT
    COUNT(*)                                                       AS totalRecipients,
    SUM(CASE WHEN nome IS NULL OR nome = '' THEN 1 ELSE 0 END)     AS missingNameRemaining,
    SUM(CASE WHEN NISS IS NULL OR NISS = '' THEN 1 ELSE 0 END)     AS missingNissRemaining
FROM dbo.DESTINATARIO
WHERE indActivo = 1;

PRINT '';
PRINT 'Recipients still without a name (no worker and no employer to copy from).';
PRINT 'These need to be corrected by hand -- there is no source for their name:';

SELECT TOP 50 id, trabalhador_fk, entidade_fk, TIN, dataCriacao, utilizadorCriacao
FROM dbo.DESTINATARIO
WHERE indActivo = 1 AND (nome IS NULL OR nome = '')
ORDER BY id;

PRINT '';
PRINT 'DONE. Re-open the payment order PDF -- the NISS and name columns should now be filled.';

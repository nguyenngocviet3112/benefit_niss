-- ============================================================================
-- Backfill COMPONENTERECEITA_REGISTO.InstitutionId for existing Receita rows.
--
-- Background: the "Registar Receita" screen never wrote InstitutionId until
-- the 2026-07-28 fix (Institution dropdown added to componente-receita,
-- defaults to "INSS"). Every Receita registered before that fix has
-- InstitutionId = NULL, which makes the CE_OSS_Global report's monthly
-- Receita columns silently exclude those rows (report filters by
-- InstitutionId == request.institution, a non-nullable match).
--
-- This script backfills only the NULL rows to the "INSS" institution (looked
-- up by name, not a hardcoded id, so it is safe to run as-is on any
-- environment -- Local/Staging/Production -- regardless of the actual id
-- INSS happens to have there).
--
-- Idempotent: only touches rows where InstitutionId IS NULL, safe to re-run.
-- BACK UP THE DATABASE BEFORE RUNNING ON STAGING/PRODUCTION.
-- ============================================================================

SET NOCOUNT ON;

DECLARE @inssId INT = (SELECT id FROM INSTITUTION WHERE nome = 'INSS' AND indActivo = 1);

IF @inssId IS NULL
BEGIN
    RAISERROR('No active INSTITUTION row named ''INSS'' found -- aborting, check institution naming on this environment before running.', 16, 1);
    RETURN;
END

PRINT 'Backfilling COMPONENTERECEITA_REGISTO.InstitutionId -> INSS (id=' + CAST(@inssId AS VARCHAR) + ') for NULL rows...';

UPDATE COMPONENTERECEITA_REGISTO
SET InstitutionId = @inssId
WHERE InstitutionId IS NULL;

PRINT 'Rows updated: ' + CAST(@@ROWCOUNT AS VARCHAR);
